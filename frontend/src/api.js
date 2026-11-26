import { createContext, useCallback, useContext, useEffect, useState } from 'react'

const base = (import.meta.env.VITE_API_BASE_URL || 'https://localhost:7003/api').replace(/\/+$/, '')

const statusMessages = {
  400: 'Сервер отклонил данные запроса.',
  401: 'Войдите в систему.',
  403: 'Недостаточно прав для этой операции.',
  404: 'Запись не найдена.',
}

export async function api(path, { method = 'GET', body, signal } = {}) {
  const json = body !== undefined && !(body instanceof FormData)
  let response
  try {
    response = await fetch(`${base}/${path}`, {
      method,
      headers: json ? { 'Content-Type': 'application/json' } : undefined,
      body: json ? JSON.stringify(body) : body,
      signal,
    })
  } catch (error) {
    if (error.name === 'AbortError') throw error
    throw new Error('Не удалось подключиться к серверу OnlineJudge.', { cause: error })
  }

  const text = await response.text()
  const contentType = response.headers.get('Content-Type') || ''
  let data = text || null
  if (text && contentType.includes('json')) {
    try {
      data = JSON.parse(text)
    } catch {
      throw new Error('Сервер вернул некорректный JSON.')
    }
  }

  if (!response.ok) {
    const validation = data?.errors && Object.values(data.errors).flat().join(' ')
    const plainText = contentType.includes('text/plain') && text.length < 300 && text
    const message = validation || data?.message || plainText || statusMessages[response.status]
      || data?.title || `Ошибка сервера (HTTP ${response.status}).`
    const error = new Error(message)
    error.status = response.status
    throw error
  }
  return data
}

export function pageId() {
  const id = new URLSearchParams(window.location.search).get('id')
  return id && /^\d+$/.test(id) && Number(id) > 0 && Number(id) <= 2147483647
    ? String(Number(id))
    : null
}

// Key the result by its request so a changed filter never displays the previous response.
export function useResource(path) {
  const [revision, setRevision] = useState(0)
  const [result, setResult] = useState(null)
  const reload = useCallback(() => setRevision(value => value + 1), [])

  useEffect(() => {
    if (!path) return
    const controller = new AbortController()
    function finish(data, error) {
      if (!controller.signal.aborted) setResult({ path, revision, data, error })
    }
    api(path, { signal: controller.signal }).then(
      data => finish(data, null),
      error => finish(null, error),
    )
    return () => controller.abort()
  }, [path, revision])

  const current = result?.path === path && result?.revision === revision
  return {
    data: current ? result.data : null,
    error: current ? result.error : null,
    loading: Boolean(path) && !current,
    reload,
  }
}

export const SessionContext = createContext(null)

export function useSession() {
  return useContext(SessionContext)
}
