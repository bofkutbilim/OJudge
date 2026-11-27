import { useState } from 'react'
import { api, useResource, useSession } from '../../api.js'
import { ResourceState } from '../../components.jsx'
import { ActionForm } from '../../forms.jsx'
import { ProblemTable } from './ProblemTable.jsx'

const difficultyRanges = { all: [-1, 100], easy: [1, 32], medium: [33, 65], hard: [66, 100], undefined: [-1, -1] }

function ArchiveFilter({ onSearch }) {
  function search(event) {
    event.preventDefault()
    const data = new FormData(event.currentTarget)
    const [min, max] = difficultyRanges[data.get('difficulty')]
    const query = new URLSearchParams({ str: data.get('problem').trim() })
    onSearch(`problem/get/filter/${min}/${max}/${data.get('solved')}?${query}`)
  }

  return (
    <section className="panel">
      <h2>Фильтр задач</h2>
      <form className="form-stack" onSubmit={search}>
        <label className="field">Задача<input name="problem" type="search" placeholder="Название задачи" /></label>
        <label className="field">Сложность
          <select name="difficulty" defaultValue="all">
            <option value="all">Любая</option><option value="easy">Easy</option>
            <option value="medium">Medium</option><option value="hard">Hard</option><option value="undefined">Undefined</option>
          </select>
        </label>
        <label className="field">Решение
          <select name="solved" defaultValue="none">
            <option value="none">Все задачи</option><option value="true">Решённые</option><option value="false">Нерешённые</option>
          </select>
        </label>
        <button type="submit">Найти</button>
      </form>
    </section>
  )
}

function TopSolvers() {
  const users = useResource('user/gettop')
  return (
    <section className="panel">
      <h2><a href="/ranklist.html">Топ решателей</a></h2>
      <ResourceState resource={users} />
      {users.data && (users.data.length ? <div className="table-wrap">
        <table>
          <thead><tr><th scope="col">Пользователь</th><th scope="col">Решено</th></tr></thead>
          <tbody>{users.data.map(user => <tr key={user.id}>
            <td><a href={`/users/profile.html?id=${user.id}`}>{user.nickName}</a></td><td>{user.solved}</td>
          </tr>)}</tbody>
        </table>
      </div> : <p className="empty">Рейтинг пока пуст.</p>)}
    </section>
  )
}

export function ArchivePage() {
  const { isAdmin } = useSession()
  const [path, setPath] = useState('problem/get')
  const [adding, setAdding] = useState(false)
  const problems = useResource(path)

  function search(nextPath) {
    if (path === nextPath) problems.reload()
    else setPath(nextPath)
  }

  async function addProblem(data) {
    await api('problem/create', { method: 'POST', body: { title: data.get('title').trim(), timeLimitMilliseconds: 1000, memoryLimitKB: 16000 } })
    setAdding(false)
    problems.reload()
  }

  async function removeProblem(id) {
    await api(`problem/delete${id}`, { method: 'DELETE' })
    problems.reload()
  }

  return (
    <div className="page-grid">
      <section className="panel">
        <div className="page-heading">
          <h1>Архив задач</h1>
          {isAdmin && <button type="button" className="secondary" onClick={() => setAdding(true)}>+ Добавить задачу</button>}
        </div>
        <ResourceState resource={problems} />
        {problems.data && <ProblemTable problems={problems.data} onRemove={isAdmin ? removeProblem : undefined} />}
      </section>
      <aside className="stack"><ArchiveFilter onSearch={search} /><TopSolvers /></aside>
      {isAdmin && adding && (
        <ActionForm dialogTitle="Добавить задачу" onClose={() => setAdding(false)} onSave={addProblem} label="Добавить">
          <label className="field">Название задачи<input name="title" maxLength="50" required /></label>
        </ActionForm>
      )}
    </div>
  )
}
