import { useEffect, useId, useRef } from 'react'
import { SessionContext, useResource } from './api.js'

export function SessionProvider({ children }) {
  const { data, error, loading, reload } = useResource('user/isactive')
  const guest = error?.status === 400 && error.message === 'Not Active'
  const session = { user: data?.message === 'Active' ? data : null, loading, error: guest ? null : error, reload }
  return <SessionContext.Provider value={session}>{children}</SessionContext.Provider>
}

export function ResourceState({ resource }) {
  if (resource.loading) return <p className="loading" role="status">Загрузка…</p>
  if (resource.error) {
    return (
      <div className="resource-error" role="alert">
        <span>{resource.error.message}</span>
        <button type="button" className="secondary" onClick={resource.reload}>Повторить</button>
      </div>
    )
  }
  return null
}

export function Modal({ title, onClose, busy = false, children }) {
  const ref = useRef(null)
  const titleId = useId()
  useEffect(() => {
    const dialog = ref.current
    dialog.showModal()
    return () => dialog.close()
  }, [])

  return (
    <dialog ref={ref} className="modal" aria-labelledby={titleId}
      onCancel={event => {
        event.preventDefault()
        if (!busy) onClose()
      }}>
      <div className="page-heading">
        <h2 id={titleId}>{title}</h2>
        <button type="button" className="modal-close secondary" aria-label="Закрыть" onClick={onClose} disabled={busy}>×</button>
      </div>
      {children}
    </dialog>
  )
}
