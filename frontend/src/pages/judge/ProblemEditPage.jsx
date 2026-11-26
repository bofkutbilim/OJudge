import { api, pageId, useResource } from '../../api.js'
import { ResourceState } from '../../components.jsx'
import { ActionForm, RemoveButton } from '../../forms.jsx'

function ProblemSettings({ problem }) {
  async function update(data) {
    const body = {}
    const title = data.get('title').trim()
    if (title && title !== problem.title) body.title = title
    for (const name of ['timeLimitMilliseconds', 'memoryLimitKB', 'difficulty']) {
      const value = data.get(name)
      if (value !== '' && Number(value) !== problem[name]) body[name] = Number(value)
    }
    await api(`problem/update${problem.id}`, { method: 'PUT', body })
    window.location.assign('/archieve.html')
  }

  return (
    <section className="panel">
      <h1>Редактирование задачи</h1>
      <p><a href={`/problem.html?id=${problem.id}`}>{problem.id}. {problem.title}</a></p>
      <ActionForm onSave={update} label="Сохранить">
        <label className="field">Название задачи<input name="title" defaultValue={problem.title} maxLength="50" required /></label>
        <label className="field">Время, мс<input type="number" name="timeLimitMilliseconds" min="1" max="20000" defaultValue={problem.timeLimitMilliseconds ?? ''} /></label>
        <label className="field">Память, КБ<input type="number" name="memoryLimitKB" min="1" max="256000" defaultValue={problem.memoryLimitKB ?? ''} /></label>
        <label className="field">Сложность от 1 до 100<input type="number" name="difficulty" min="1" max="100" placeholder="Оставить текущую" /></label>
      </ActionForm>
    </section>
  )
}

function ProblemTests({ id }) {
  const tests = useResource(`problem/get/tests${id}`)

  async function addTest(data) {
    await api(`problem/addtest${id}`, { method: 'PUT', body: {
      input: data.get('input'), output: data.get('output'), point: Number(data.get('point'))
    } })
    tests.reload()
  }

  async function removeTest(testId) {
    await api(`test/delete${testId}`, { method: 'DELETE' })
    tests.reload()
  }

  return (
    <section className="panel">
      <h2>Тесты</h2>
      <ResourceState resource={tests} />
      {tests.data && (tests.data.length ? <div className="table-wrap">
        <table>
          <thead><tr><th scope="col">№</th><th scope="col">Ввод</th><th scope="col">Вывод</th><th scope="col">Баллы</th><th scope="col" aria-label="Действия" /></tr></thead>
          <tbody>{tests.data.map((test, index) => <tr key={test.id}>
            <td>{index + 1}</td><td><pre>{test.input}</pre></td><td><pre>{test.output}</pre></td><td>{test.point}</td>
            <td><RemoveButton label={`Удалить тест №${index + 1}`} question={`Удалить тест №${index + 1}?`} onRemove={() => removeTest(test.id)} /></td>
          </tr>)}</tbody>
        </table>
      </div> : <p className="empty">К задаче пока не добавили тесты.</p>)}
      <h3>Новый тест</h3>
      <ActionForm onSave={addTest} label="Добавить тест" resetOnSuccess>
        <label className="field">Ввод<textarea name="input" rows="3" required /></label>
        <label className="field">Вывод<textarea name="output" rows="3" required /></label>
        <label className="field">Баллы<input name="point" type="number" required /></label>
      </ActionForm>
    </section>
  )
}

function ProblemTopics({ id }) {
  const assigned = useResource(`problem/gettopics/byproblem${id}`)
  const topics = useResource('topic/get')
  const assignedIds = new Set(assigned.data?.map(topic => topic.id))
  const choices = topics.data?.filter(topic => !assignedIds.has(topic.id))

  async function addTopic(data) {
    await api(`problem/addtopic${id}/${data.get('topic')}`, { method: 'PUT' })
    assigned.reload()
  }

  async function removeTopic(topicId) {
    await api(`problem/deletetopic${id}/${topicId}`, { method: 'DELETE' })
    assigned.reload()
  }

  return (
    <section className="panel">
      <h2>Темы задачи</h2>
      <ResourceState resource={assigned} />
      {assigned.data && (assigned.data.length ? <div className="table-wrap">
        <table>
          <thead><tr><th scope="col">№</th><th scope="col">Тема</th><th scope="col" aria-label="Действия" /></tr></thead>
          <tbody>{assigned.data.map((topic, index) => <tr key={topic.id}>
            <td>{index + 1}</td><td><a href={`/topic.html?id=${topic.id}`}>{topic.title}</a></td>
            <td><RemoveButton label={`Убрать тему «${topic.title}»`} question={`Убрать «${topic.title}» из тем этой задачи?`} onRemove={() => removeTopic(topic.id)} /></td>
          </tr>)}</tbody>
        </table>
      </div> : <p className="empty">У задачи пока нет тем.</p>)}
      <ResourceState resource={topics} />
      {assigned.data && choices && (choices.length ? <ActionForm onSave={addTopic} label="Добавить тему">
        <label className="field">Тема<select name="topic" required>
          {choices.map(topic => <option key={topic.id} value={topic.id}>{topic.title}</option>)}
        </select></label>
      </ActionForm> : <p className="empty">Нет других тем для добавления.</p>)}
    </section>
  )
}

export function ProblemEditPage() {
  const id = pageId()
  const problem = useResource(id ? `problem/get${id}` : null)
  if (!problem.data) return <section className="panel">
    <h1>{id ? 'Редактирование задачи' : 'Задача не выбрана'}</h1>
    {id && <ResourceState resource={problem} />}<a href="/archieve.html">В архив задач</a>
  </section>
  return <div className="judge-edit-grid">
    <ProblemSettings problem={problem.data} />
    <ProblemTests id={id} />
    <ProblemTopics id={id} />
  </div>
}
