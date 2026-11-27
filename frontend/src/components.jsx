import { useEffect, useId, useRef } from 'react'
import { clearSession, SessionContext, useResource, useSession, useStoredSession } from './api.js'

export function SessionProvider({ children }) {
  const stored = useStoredSession()
  const { data, error, loading, reload } = useResource(stored ? 'user/isactive' : null)
  useEffect(() => {
    if (!stored) return
    const remaining = Math.max(0, Date.parse(stored.expiresAt) - Date.now())
    const timer = window.setTimeout(() => clearSession(stored.accessToken), remaining)
    return () => window.clearTimeout(timer)
  }, [stored])
  useEffect(() => {
    function restore(event) {
      if (event.persisted) window.location.reload()
    }
    window.addEventListener('pageshow', restore)
    return () => window.removeEventListener('pageshow', restore)
  }, [])
  const user = data?.message === 'Active' ? data : null
  const session = { user, isAdmin: user?.userRole === 'admin', loading, error, reload }
  return <SessionContext.Provider value={session}>{children}</SessionContext.Provider>
}

export function EditorAccess({ ownerId, children }) {
  const session = useSession()
  if (session.loading || session.error) return <ResourceState resource={session} />
  if (session.isAdmin || (ownerId && String(session.user?.userId) === ownerId)) return children
  return <section className="panel narrow">
    <h1>Доступ к редактированию</h1>
    {session.user ? <p>Недостаточно прав.</p> : <a href="/login.html">Войти в аккаунт</a>}
  </section>
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
