import { useState } from 'react'
import { Modal } from './components.jsx'

export function ActionForm({ children, onSave, label, resetOnSuccess = false, dialogTitle, onClose }) {
  const [pending, setPending] = useState(false)
  const [error, setError] = useState('')

  async function submit(event) {
    event.preventDefault()
    if (pending) return
    const form = event.currentTarget
    const data = new FormData(form)
    setPending(true)
    setError('')
    try {
      await onSave(data)
      if (resetOnSuccess) form.reset()
    } catch (cause) {
      setError(cause.message)
    } finally {
      setPending(false)
    }
  }

  const form = (
    <form onSubmit={submit} aria-busy={pending}>
      <fieldset className="form-stack form-fields" disabled={pending}>
        {children}
        {error && <p className="error" role="alert">{error}</p>}
        <button type="submit">{pending ? 'Отправляем…' : label}</button>
      </fieldset>
    </form>
  )
  return dialogTitle
    ? <Modal title={dialogTitle} onClose={onClose} busy={pending}>{form}</Modal>
    : form
}

export function RemoveButton({ label, question, onRemove }) {
  const [pending, setPending] = useState(false)
  const [error, setError] = useState('')

  async function remove() {
    if (pending || !window.confirm(question)) return
    setPending(true)
    setError('')
    try {
      await onRemove()
    } catch (cause) {
      setError(cause.message)
    } finally {
      setPending(false)
    }
  }

  return (
    <>
      <button type="button" className="icon-button danger" aria-label={label} title={label} disabled={pending} onClick={remove}>×</button>
      {error && <p className="error" role="alert">{error}</p>}
    </>
  )
}
