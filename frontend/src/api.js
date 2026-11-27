import { createContext, useCallback, useContext, useEffect, useMemo, useState, useSyncExternalStore } from 'react'

const base = (import.meta.env.VITE_API_BASE_URL || 'https://localhost:7003/api').replace(/\/+$/, '')
const sessionKey = `onlinejudge.session:${base}`
const sessionEvent = 'onlinejudge-session'

function readStored() {
  try {
    return sessionStorage.getItem(sessionKey)
  } catch {
    return null
  }
}

function parseSession(raw) {
  if (!raw) return null
  try {
    const record = JSON.parse(raw)
    return typeof record?.accessToken === 'string' && Number.isFinite(Date.parse(record.expiresAt)) ? record : null
  } catch {
    return null
  }
}

function subscribeSession(notify) {
  window.addEventListener(sessionEvent, notify)
  window.addEventListener('storage', notify)
  return () => {
    window.removeEventListener(sessionEvent, notify)
    window.removeEventListener('storage', notify)
  }
}

export function useStoredSession() {
  const raw = useSyncExternalStore(subscribeSession, readStored)
  return useMemo(() => parseSession(raw), [raw])
}

export function clearSession(token) {
  if (token && parseSession(readStored())?.accessToken === token) {
    sessionStorage.removeItem(sessionKey)
    window.dispatchEvent(new Event(sessionEvent))
  }
}

export async function loginAndOpenArchive(credentials) {
  const result = await api('user/login', { method: 'PUT', body: credentials, anonymous: true })
  if (!result?.accessToken || result.tokenType !== 'Bearer' || !Number.isFinite(Date.parse(result.expiresAt))) {
    throw new Error('Сервер не вернул токен входа.')
  }
  try {
    sessionStorage.setItem(sessionKey, JSON.stringify({ accessToken: result.accessToken, expiresAt: result.expiresAt }))
  } catch {
    throw new Error('Разрешите хранилище вкладки в браузере для входа.')
  }
  // The new document reads the saved token. Notifying this one would start a
  // session check just before navigation cancels its HTTP request.
  window.location.replace('/archieve.html')
}

export async function logout() {
  const token = parseSession(readStored())?.accessToken
  if (!token) return
  try {
    await api('user/logout', { method: 'PUT' })
  } catch (error) {
    if (error.status !== 401) throw error
  }
  clearSession(token)
}

const statusMessages = {
  400: 'Сервер отклонил данные запроса.',
  401: 'Войдите в систему.',
  403: 'Недостаточно прав для этой операции.',
  404: 'Запись не найдена.',
}

export async function api(path, { method = 'GET', body, signal, anonymous = false } = {}) {
  const json = body !== undefined && !(body instanceof FormData)
  const token = anonymous ? null : parseSession(readStored())?.accessToken
  const headers = new Headers({ Accept: 'application/json' })
  if (token) headers.set('Authorization', `Bearer ${token}`)
  if (json) headers.set('Content-Type', 'application/json')
  let response
  try {
    response = await fetch(`${base}/${path}`, {
      method,
      headers,
      body: json ? JSON.stringify(body) : body,
      signal,
    })
  } catch (error) {
    if (error.name === 'AbortError') throw error
    throw new Error('Не удалось подключиться к серверу OnlineJudge.', { cause: error })
  }

  if (response.status === 401) clearSession(token)
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
  if (text && !contentType.includes('json')) throw new Error('Ожидался JSON-ответ от API OnlineJudge.')
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
  const token = useStoredSession()?.accessToken
  const [revision, setRevision] = useState(0)
  const [result, setResult] = useState(null)
  const reload = useCallback(() => setRevision(value => value + 1), [])

  useEffect(() => {
    if (!path) return
    const controller = new AbortController()
    function finish(data, error) {
      if (!controller.signal.aborted) setResult({ path, revision, token, data, error })
    }
    // StrictMode can clean up the initial effect and immediately set it up again.
    queueMicrotask(() => {
      if (controller.signal.aborted) return
      api(path, { signal: controller.signal }).then(
        data => finish(data, null),
        error => finish(null, error),
      )
    })
    return () => controller.abort()
  }, [path, revision, token])

  const current = result?.path === path && result?.revision === revision && result?.token === token
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
