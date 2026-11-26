import { useState } from 'react'
import { api, pageId, useResource } from '../api.js'
import { ResourceState } from '../components.jsx'
import { ActionForm, RemoveButton } from '../forms.jsx'
import { ProblemTable } from './judge/ProblemTable.jsx'
import { StatementText } from './judge/StatementText.jsx'
import './topics/topics.css'

const topicsUrl = '/topics.html'

function TopicsLink() {
  return <a className="topics-back" href={topicsUrl}>← Все темы</a>
}

function TopicGroup({ group, index, reload }) {
  async function remove(path) {
    await api(path, { method: 'DELETE' })
    reload()
  }
  return (
    <section className="panel topic-group">
      <div className="topic-group-heading">
        <h2>{index + 1}. {group.title}</h2>
        <div className="actions">
          <a className="button secondary" href={`/maintopics/addtopic.html?id=${group.id}`}>Добавить подтему</a>
          <a className="icon-button" href={`/maintopics/edit.html?id=${group.id}`} aria-label={`Изменить тему «${group.title}»`} title="Изменить тему">✎</a>
          <RemoveButton label={`Удалить тему «${group.title}»`} question={`Удалить тему «${group.title}»?`}
            onRemove={() => remove(`maintopic/delete${group.id}`)} />
        </div>
      </div>
      {group.topics.length === 0 ? <p className="empty">В этой теме пока нет подтем.</p> : (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th scope="col">№</th><th scope="col">Название</th>
                <th scope="col">Undefined</th><th scope="col">Easy</th><th scope="col">Medium</th><th scope="col">Hard</th>
                <th scope="col">Действия</th>
              </tr>
            </thead>
            <tbody>
              {group.topics.map((topic, position) => (
                <tr key={topic.id}>
                  <td>{position + 1}</td>
                  <td><a href={`/topic.html?id=${topic.id}`}>{topic.title}</a></td>
                  <td>{topic.undefined}</td><td>{topic.easy}</td><td>{topic.medium}</td><td>{topic.hard}</td>
                  <td>
                    <div className="table-actions">
                      <a className="icon-button" href={`/topics/edit.html?id=${topic.id}`} aria-label={`Изменить подтему «${topic.title}»`} title="Изменить подтему">✎</a>
                      <RemoveButton label={`Удалить подтему «${topic.title}»`} question={`Удалить подтему «${topic.title}»?`}
                        onRemove={() => remove(`topic/delete${topic.id}`)} />
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  )
}

export function TopicsPage() {
  const groups = useResource('maintopic/get')
  const [creating, setCreating] = useState(false)

  async function addTopic(fields) {
    const title = fields.get('title').trim()
    if (!title) throw new Error('Введите название темы.')
    await api('maintopic/create', { method: 'POST', body: { title } })
    setCreating(false)
    groups.reload()
  }

  return (
    <div className="stack">
      <div className="page-heading">
        <h1>Темы</h1>
        <button type="button" className="button" onClick={() => setCreating(true)}>Добавить тему</button>
      </div>
      <ResourceState resource={groups} />
      {groups.data?.length === 0 && <p className="panel empty">Тем пока нет.</p>}
      {groups.data?.map((group, index) => <TopicGroup key={group.id} group={group} index={index} reload={groups.reload} />)}
      {creating && (
        <ActionForm dialogTitle="Добавить тему" onClose={() => setCreating(false)} onSave={addTopic} label="Добавить">
          <label className="field">Название темы<input name="title" required /></label>
        </ActionForm>
      )}
    </div>
  )
}

export function TopicPage() {
  const id = pageId()
  const topic = useResource(id && `topic/get${id}`)
  const problems = useResource(id && `problem/get/bytopic${id}`)

  return (
    <div className="stack">
      <TopicsLink />
      <section className="panel">
        <h1>{topic.data?.title || 'Подтема'}</h1>
        {!id && <p className="error">Подтема не выбрана.</p>}
        {id && <ResourceState resource={topic} />}
        {topic.data?.description && <StatementText text={topic.data.description} />}
      </section>
      {topic.data && (
        <section className="panel">
          <h2>Список задач</h2>
          <ResourceState resource={problems} />
          {problems.data && <ProblemTable problems={problems.data} />}
        </section>
      )}
    </div>
  )
}

function TopicForm({ item, editingMain, addingSubtopic }) {
  async function save(fields) {
    const title = fields.get('title').trim()
    const body = { title }
    if (!editingMain) {
      body.shortTitle = fields.get('shortTitle').trim()
      body.description = fields.get('description')
    }
    if (!title || (!editingMain && !body.shortTitle)) {
      throw new Error('Заполните обязательные поля.')
    }
    if (!editingMain && !addingSubtopic) {
      if (item.description && !body.description.trim()) {
        throw new Error('Сервер поддерживает только замену описания. Введите текст описания.')
      }
      for (const field of Object.keys(body)) {
        if (body[field] === (item[field] ?? '')) delete body[field]
      }
    }
    const path = addingSubtopic ? 'topic/post' : editingMain ? 'maintopic/update' : 'topic/update'
    const saved = await api(`${path}${item.id}`, { method: addingSubtopic ? 'POST' : 'PUT', body })
    if (!saved) throw new Error('Тема больше не существует.')
    window.location.assign(topicsUrl)
  }

  return (
    <ActionForm label={addingSubtopic ? 'Добавить' : 'Сохранить'} onSave={save}>
      <label className="field">
        {editingMain ? 'Название темы' : 'Название подтемы'}
        <input name="title" defaultValue={addingSubtopic ? '' : item.title} required />
      </label>
      {!editingMain && <>
        <label className="field">Короткое обозначение<input name="shortTitle" defaultValue={addingSubtopic ? '' : item.shortTitle} required /></label>
        <label className="field">Описание<textarea name="description" defaultValue={addingSubtopic ? '' : (item.description || '')} rows={4} /></label>
      </>}
    </ActionForm>
  )
}

function TopicEditor({ title, editingMain = false, addingSubtopic = false }) {
  const id = pageId()
  const collection = editingMain || addingSubtopic ? 'maintopic' : 'topic'
  const resource = useResource(id && `${collection}/get${id}`)

  return (
    <div className="narrow stack">
      <TopicsLink />
      <section className="panel stack">
        <h1>{title}</h1>
        {!id && <p className="error">{collection === 'maintopic' ? 'Тема не выбрана.' : 'Подтема не выбрана.'}</p>}
        {id && <ResourceState resource={resource} />}
        {resource.data && <>
          <p className="topic-editor-context">{resource.data.id}. {resource.data.title}</p>
          <TopicForm key={resource.data.id} item={resource.data} editingMain={editingMain} addingSubtopic={addingSubtopic} />
        </>}
      </section>
    </div>
  )
}

export function TopicEditPage() {
  return <TopicEditor title="Редактировать подтему" />
}

export function MainTopicEditPage() {
  return <TopicEditor title="Редактировать тему" editingMain />
}

export function MainTopicAddPage() {
  return <TopicEditor title="Добавить подтему" addingSubtopic />
}
