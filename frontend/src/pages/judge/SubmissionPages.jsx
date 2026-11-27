import { lazy, Suspense, useState } from 'react'
import { pageId, useResource, useSession } from '../../api.js'
import { ResourceState } from '../../components.jsx'
import { LanguageOptions, SubmittedAt, Verdict } from './SubmissionInfo.jsx'

const SubmissionCode = lazy(() => import('./SubmissionCode.jsx'))

function SubmissionAccess({ session }) {
  return <section className="panel narrow">
    <h1>Посылки</h1>
    <ResourceState resource={session} />
    {!session.loading && !session.error && <p><a href="/login.html">Войдите</a>, чтобы посмотреть посылки.</p>}
  </section>
}

function UserLink({ item }) {
  return <a href={`/users/profile.html?id=${item.userId}`}>{item.userNickName}</a>
}

function ProblemLink({ item }) {
  return <a href={`/problem.html?id=${item.problemId}`}>{item.problemId}. {item.problemTitle}</a>
}

function SubmissionTable({ items }) {
  if (!items.length) return <p className="empty">Посылок не найдено.</p>
  return (
    <div className="table-wrap">
      <table>
        <thead><tr>{['№', 'Кто', 'Задача', 'Вердикт', 'Баллы', 'Тест', 'Время', 'Память', 'Язык', 'Когда'].map(label =>
          <th key={label} scope="col">{label}</th>
        )}</tr></thead>
        <tbody>{items.map(item =>
          <tr key={item.id}>
            <td><a href={`/submission.html?id=${item.id}`}>{item.id}</a></td>
            <td><UserLink item={item} /></td><td><ProblemLink item={item} /></td>
            <td><Verdict title={item.verdictTitle} /></td><td>{item.points}</td><td>{item.lastTest}</td>
            <td>{item.timeLimitMilliseconds} мс</td><td>{item.memoryLimitKB} КБ</td>
            <td>{item.programmingLanguageTitle}</td><td><SubmittedAt value={item.received} /></td>
          </tr>
        )}</tbody>
      </table>
    </div>
  )
}

export function SubmissionsPage() {
  const session = useSession()
  const [path, setPath] = useState('submission/get')
  const submissions = useResource(session.user ? path : null)
  if (!session.user) return <SubmissionAccess session={session} />

  function search(event) {
    event.preventDefault()
    const query = new URLSearchParams(new FormData(event.currentTarget))
    const next = `submission/get/filter?${query}`
    if (next === path) submissions.reload()
    else setPath(next)
  }

  return (
    <div className="page-grid">
      <section className="panel">
        <div className="page-heading"><h1>{session.isAdmin ? 'Все посылки' : 'Мои посылки'}</h1></div>
        <ResourceState resource={submissions} />
        {submissions.data && <SubmissionTable items={submissions.data} />}
      </section>
      <aside className="panel">
        <h2>Фильтр посылок</h2>
        <form className="form-stack" onSubmit={search}>
          <label className="field">Задача<input type="search" name="problem" placeholder="Название задачи" /></label>
          <label className="field">Вердикт
            <select name="verdict" defaultValue="none">
              <option value="none">Любой</option><option>Wrong Answer</option><option>Accepted</option><option>Compile Error</option>
            </select>
          </label>
          <label className="field">Язык
            <select name="language" defaultValue="none"><option value="none">Все языки</option><LanguageOptions /></select>
          </label>
          {session.isAdmin && <label className="field">Пользователь<input name="user" placeholder="Никнейм" /></label>}
          <button type="submit">Найти</button>
        </form>
      </aside>
    </div>
  )
}

export function SubmissionPage() {
  const session = useSession()
  const id = pageId()
  const detail = useResource(session.user && id ? `submission/${id}` : null)
  if (!session.user) return <SubmissionAccess session={session} />
  const item = detail.data?.summary
  return (
    <section className="panel">
      <div className="page-heading"><h1>Посылка{id && ` №${id}`}</h1></div>
      <ResourceState resource={detail} />
      {!id && <p className="empty">Посылка не найдена.</p>}
      {item && <dl className="judge-details">
        <dt>Вердикт</dt><dd><Verdict title={item.verdictTitle} /></dd>
        <dt>Пользователь</dt><dd><UserLink item={item} /></dd>
        <dt>Задача</dt><dd><ProblemLink item={item} /></dd>
        <dt>Язык</dt><dd>{item.programmingLanguageTitle}</dd>
        <dt>Дата отправки</dt><dd><SubmittedAt value={item.received} /></dd>
        <dt>Баллы</dt><dd>{item.points}</dd><dt>Тест</dt><dd>{item.lastTest}</dd>
        <dt>Время</dt><dd>{item.timeLimitMilliseconds} мс</dd><dt>Память</dt><dd>{item.memoryLimitKB} КБ</dd>
      </dl>}
      {item && <>
        <h2>Исходный код</h2>
        <Suspense fallback={<pre className="submission-source"><code>{detail.data.codeText}</code></pre>}>
          <SubmissionCode languageTitle={item.programmingLanguageTitle} code={detail.data.codeText} />
        </Suspense>
      </>}
      <a href="/submissions.html">{session.isAdmin ? 'Все посылки' : 'Мои посылки'}</a>
    </section>
  )
}
