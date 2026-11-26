import { useState } from 'react'
import { api, pageId, useResource, useSession } from '../../api.js'
import { ResourceState } from '../../components.jsx'
import { ActionForm } from '../../forms.jsx'
import { StatementText } from './StatementText.jsx'
import { LanguageOptions, SubmittedAt, Verdict } from './SubmissionInfo.jsx'

function SendSolution({ id }) {
  const session = useSession()

  async function send(data) {
    // The API describes the language in the query; the legacy multipart form also includes it.
    const language = new URLSearchParams({ ProgrammingLanguageTitle: data.get('ProgrammingLanguageTitle') })
    await api(`submission/submit${id}?${language}`, { method: 'POST', body: data })
    window.location.assign('/submissions.html')
  }

  return (
    <section className="panel">
      <h2>Отправить решение</h2>
      <ResourceState resource={session} />
      {session.user && <ActionForm onSave={send} label="Отправить">
        <label className="field">Язык программирования
          <select name="ProgrammingLanguageTitle" required><LanguageOptions /></select>
        </label>
        <label className="field">Файл решения<input type="file" name="File" required /></label>
      </ActionForm>}
      {!session.loading && !session.user && !session.error && <p><a href="/login.html">Войдите</a>, чтобы отправить решение.</p>}
    </section>
  )
}

function RecentSubmissions({ id }) {
  const submissions = useResource(`submission/getmy${id}`)
  return (
    <section className="panel">
      <h2>Мои посылки</h2>
      <ResourceState resource={submissions} />
      {submissions.data && (submissions.data.length ? <div className="table-wrap">
        <table>
          <thead><tr><th scope="col">№</th><th scope="col">Когда</th><th scope="col">Вердикт</th></tr></thead>
          <tbody>{submissions.data.map(item =>
            <tr key={item.id}>
              <td><a href={`/submission.html?id=${item.id}`}>{item.id}</a></td>
              <td><SubmittedAt value={item.received} /></td><td><Verdict title={item.verdictTitle} /></td>
            </tr>
          )}</tbody>
        </table>
      </div> : <p className="empty">Вы ещё не отправляли решений этой задачи.</p>)}
    </section>
  )
}

export function ProblemPage() {
  const id = pageId()
  const problem = useResource(id ? `problem/get${id}` : null)
  const [adding, setAdding] = useState(false)
  const item = problem.data

  async function addSection(data) {
    await api(`problem/addsection${id}`, { method: 'PUT', body: Object.fromEntries(data) })
    setAdding(false)
    problem.reload()
  }

  if (!id) return <section className="panel"><h1>Задача не выбрана</h1><a href="/archieve.html">В архив задач</a></section>
  if (!item) return <section className="panel"><h1>Задача №{id}</h1><ResourceState resource={problem} /><a href="/archieve.html">В архив задач</a></section>
  return (
    <div className="page-grid">
      <article className="panel">
        <div className="page-heading"><h1>{item.id}. {item.title}</h1></div>
        {item.problemInformations?.map(section => <section className="judge-section" key={section.id}>
          {section.sectionName && <h2>{section.sectionName}</h2>}
          <StatementText text={section.sectionText} />
        </section>)}
        <button type="button" className="secondary" onClick={() => setAdding(true)}>+ Добавить секцию</button>
      </article>
      <aside className="stack">
        <section className="panel">
          <h2>Ограничения</h2>
          <dl className="judge-details">
            <dt>Время</dt><dd>{item.timeLimitMilliseconds == null ? '—' : `${item.timeLimitMilliseconds / 1000} с`}</dd>
            <dt>Память</dt><dd>{item.memoryLimitKB == null ? '—' : `${item.memoryLimitKB / 1000} МБ`}</dd>
          </dl>
          {!!item.topics?.length && <>
            <h2>Темы</h2>
            <ul className="judge-topics">{item.topics.map(topic => <li key={topic.id}><a href={`/topic.html?id=${topic.id}`}>{topic.title}</a></li>)}</ul>
          </>}
        </section>
        <SendSolution id={id} />
        <RecentSubmissions id={id} />
      </aside>
      {adding && (
        <ActionForm dialogTitle="Добавить секцию" onClose={() => setAdding(false)} onSave={addSection} label="Добавить">
          <label className="field">Название секции<input name="sectionName" required /></label>
          <label className="field">Текст секции<textarea name="sectionText" rows="6" required /></label>
        </ActionForm>
      )}
    </div>
  )
}
