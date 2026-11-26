import { createElement, Fragment, useMemo, useState } from 'react'

// SectionText can contain formatted statements and sample tables saved by the old editor.
const tags = new Set('a b blockquote br caption code col colgroup div em h2 h3 h4 hr i img kbd li ol p pre s samp small span strong sub sup table tbody td th thead tr u ul var'.split(' '))
const omit = new Set(['script', 'style', 'template', 'iframe', 'object', 'svg', 'math'])

function documentUrl(value) {
  if (!value) return undefined
  try {
    const url = new URL(value, window.location.href)
    return ['https:', 'http:'].includes(url.protocol) ? url.href : undefined
  } catch {
    return undefined
  }
}

function CopyableCell({ tag, children, ...props }) {
  const [status, setStatus] = useState('')
  async function copy(event) {
    const text = event.currentTarget.previousElementSibling.innerText
    try {
      await navigator.clipboard.writeText(text)
      setStatus('Скопировано')
    } catch {
      setStatus('Не удалось скопировать')
    }
  }
  return createElement(tag, props, <div>{children}</div>,
    <button type="button" className="secondary judge-copy" onClick={copy}>Копировать</button>,
    <span role="status">{status}</span>)
}

function renderNode(node, key) {
  if (node.nodeType === Node.TEXT_NODE) return node.textContent
  if (node.nodeType !== Node.ELEMENT_NODE) return null
  const tag = node.localName
  if (omit.has(tag)) return null
  const children = Array.from(node.childNodes, renderNode)
  if (!tags.has(tag)) return <Fragment key={key}>{children}</Fragment>
  if (tag === 'br' || tag === 'hr' || tag === 'col') return createElement(tag, { key })
  if (tag === 'img') {
    const src = documentUrl(node.getAttribute('src'))
    return src ? <img key={key} src={src} alt={node.getAttribute('alt') || ''} loading="lazy" /> : null
  }

  const props = { key }
  if (tag === 'a') props.href = documentUrl(node.getAttribute('href'))
  if (tag === 'td' || tag === 'th') {
    props.colSpan = node.colSpan
    props.rowSpan = node.rowSpan
    if (/^\s*copyCell\(this\);?\s*$/i.test(node.getAttribute('onclick') || '')) {
      return <CopyableCell key={key} tag={tag} colSpan={props.colSpan} rowSpan={props.rowSpan}>{children}</CopyableCell>
    }
  }
  return createElement(tag, props, children)
}

export function StatementText({ text = '' }) {
  const content = useMemo(() => {
    if (!text || !/<[a-z][^>]*>/i.test(text)) return text
    const template = document.createElement('template')
    template.innerHTML = text
    return Array.from(template.content.childNodes, renderNode)
  }, [text])
  return <div className="judge-statement">{content}</div>
}
