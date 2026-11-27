import { useEffect, useState } from 'react'
import { api, login, pageId, useResource, useSession } from '../api.js'
import { ResourceState } from '../components.jsx'
import './account/profile.css'

function AuthenticationForm({ register = false }) {
  const { user, loading, error: sessionError } = useSession()
  const [busy, setBusy] = useState(false)
  const [created, setCreated] = useState(false)
  const [error, setError] = useState('')
  const disabled = busy || created || loading || Boolean(user || sessionError)

  useEffect(() => {
    if (user) window.location.replace('/archieve.html')
  }, [user])

  async function submit(event) {
    event.preventDefault()
    if (disabled) return
    const fields = new FormData(event.currentTarget)
    const credentials = { nickName: fields.get('nickName'), password: fields.get('password') }
    setBusy(true)
    setError('')

    if (register) {
      try {
        await api('user/create', {
          method: 'POST',
          body: { ...credentials, email: fields.get('email') },
        })
        setCreated(true)
      } catch (err) {
        setError(`Не удалось зарегистрироваться. ${err.message}`)
        setBusy(false)
        return
      }
    }

    try {
      await login(credentials)
      window.location.assign('/archieve.html')
    } catch (err) {
      const message = err.status === 401 ? 'Проверьте никнейм и пароль.' : err.message
      setError(`${register ? 'Аккаунт создан. Войдите в него снова.' : 'Не удалось войти.'} ${message}`)
      setBusy(false)
    }
  }

  return (
    <section className="panel narrow">
      <h1 className="page-heading">{register ? 'Регистрация' : 'Авторизация'}</h1>
      <form className="form-stack" onSubmit={submit}>
        {register && (
          <label className="field">
            Электронная почта
            <input type="email" name="email" autoComplete="email" required />
          </label>
        )}
        <label className="field">
          Никнейм
          <input name="nickName" autoComplete="username" maxLength={50} required />
        </label>
        <label className="field">
          Пароль
          <input type="password" name="password" autoComplete={register ? 'new-password' : 'current-password'} maxLength={50} required />
        </label>
        {error && <p className="error" role="alert">{error}</p>}
        <div className="actions">
          <button type="submit" disabled={disabled}>
            {register ? 'Зарегистрироваться' : 'Войти'}
          </button>
          <a className="button secondary" href={register ? '/login.html' : '/signup.html'}>
            {register ? 'Войти' : 'Регистрация'}
          </a>
        </div>
      </form>
    </section>
  )
}

export function LoginPage() {
  return <AuthenticationForm />
}

export function SignupPage() {
  return <AuthenticationForm register />
}

function AffiliationLink({ kind, id, title }) {
  if (!id || !title) return '—'
  return <a href={`/${kind}.html?id=${id}`}>{title}</a>
}

export function RanklistPage() {
  const { user: viewer, isAdmin } = useSession()
  const ranking = useResource('user/get')
  return (
    <section className="panel">
      <h1 className="page-heading">Рейтинг участников</h1>
      <ResourceState resource={ranking} />
      {ranking.data && (
        <div className="table-wrap">
          <table className="ranking-table">
            <thead>
              <tr>
                <th scope="col">Место</th>
                <th scope="col">Никнейм</th>
                <th scope="col">Пользователь</th>
                <th scope="col">Организация</th>
                <th scope="col">Решено задач</th>
                <th scope="col">Отправки</th>
                {viewer && <th scope="col"><span className="account-sr-only">Редактирование</span></th>}
              </tr>
            </thead>
            <tbody>
              {ranking.data.map((user, index) => (
                <tr key={user.id}>
                  <td>{index + 1}</td>
                  <td><a href={`/users/profile.html?id=${user.id}`}>{user.nickName}</a></td>
                  <td>{[user.name, user.surName].filter(Boolean).join(' ') || '—'}</td>
                  <td><AffiliationLink kind="organization" id={user.organizationId} title={user.organization} /></td>
                  <td>{user.solved}</td>
                  <td>{user.submitted}</td>
                  {viewer && <td>
                    {(isAdmin || viewer.userId === user.id) && <a href={`/users/edit.html?id=${user.id}`} aria-label={`Редактировать профиль ${user.nickName}`}>
                      Изменить
                    </a>}
                  </td>}
                </tr>
              ))}
              {ranking.data.length === 0 && <tr><td className="empty" colSpan={viewer ? 7 : 6}>Пока нет участников.</td></tr>}
            </tbody>
          </table>
        </div>
      )}
    </section>
  )
}

function AffiliationPage({ kind, heading, missing }) {
  const id = pageId()
  const resource = useResource(id ? `${kind}/get` : null)
  const entry = resource.data?.find((item) => String(item.id) === id)
  return (
    <section className="panel narrow">
      <h1 className="page-heading">{entry?.title || heading}</h1>
      <ResourceState resource={resource} />
      {(!id || (resource.data && !entry)) && <p className="empty">{missing}</p>}
      {entry?.description && <p>{entry.description}</p>}
      <a href="/ranklist.html">К рейтингу участников</a>
    </section>
  )
}

export function CountryPage() {
  return <AffiliationPage kind="country" heading="Страна" missing="Страна не найдена." />
}

export function OrganizationPage() {
  return <AffiliationPage kind="organization" heading="Организация" missing="Организация не найдена." />
}

export function ProfilePage() {
  const id = pageId()
  const { user: viewer, isAdmin } = useSession()
  const profile = useResource(id ? `user/get${id}` : null)
  const user = profile.data
  const fullName = user && [user.name, user.surName].filter(Boolean).join(' ')
  return (
    <section className="panel account-profile">
      {!user && <h1 className="page-heading">Профиль участника</h1>}
      {!id && <p className="empty">Участник не выбран.</p>}
      <ResourceState resource={profile} />
      {user && (
        <div className="profile-overview">
          <img className="profile-avatar" src="/default.webp" width={200} height={200} alt="Аватар участника" />
          <div className="profile-details">
            <p className="profile-role">{user.role}</p>
            <h1 className="page-heading">{user.nickName}</h1>
            {fullName && <p>{fullName}</p>}
            <dl className="profile-facts">
              <dt>Страна</dt>
              <dd><AffiliationLink kind="country" id={user.countryId} title={user.countryTitle} /></dd>
              <dt>Организация</dt>
              <dd><AffiliationLink kind="organization" id={user.organizationId} title={user.organizationTitle} /></dd>
              <dt>Решено задач</dt><dd>{user.solved}</dd>
              <dt>Посылки</dt><dd>{user.submitted}</dd>
            </dl>
            {(isAdmin || viewer?.userId === user.id) && (
              <a className="button secondary" href={`/users/edit.html?id=${user.id}`}>Редактировать профиль</a>
            )}
          </div>
        </div>
      )}
    </section>
  )
}

function AffiliationSelect({ label, name, items, currentId }) {
  return (
    <label className="field">
      {label}
      <select name={name} defaultValue={currentId ?? ''}>
        {currentId == null && <option value="">Не указана</option>}
        {items.map((item) => <option key={item.id} value={item.id}>{item.title}</option>)}
      </select>
    </label>
  )
}

function ProfileForm({ user, countries, organizations }) {
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState('')
  const profileUrl = `/users/profile.html?id=${user.id}`

  async function save(event) {
    event.preventDefault()
    if (saving) return
    const fields = new FormData(event.currentTarget)
    setSaving(true)
    setError('')
    try {
      await api(`user/update${user.id}`, {
        method: 'PUT',
        body: {
          name: fields.get('name'),
          surName: fields.get('surName'),
          countryId: Number(fields.get('countryId')),
          organizationId: Number(fields.get('organizationId')),
        },
      })
      window.location.assign(profileUrl)
    } catch (err) {
      setError(`Не удалось сохранить профиль. ${err.message}`)
      setSaving(false)
    }
  }

  return (
    <form className="form-stack" onSubmit={save}>
      <p>{user.id}. {user.nickName}</p>
      <label className="field">
        Имя
        <input name="name" autoComplete="given-name" defaultValue={user.name ?? ''} />
      </label>
      <label className="field">
        Фамилия
        <input name="surName" autoComplete="family-name" defaultValue={user.surName ?? ''} />
      </label>
      <AffiliationSelect label="Страна" name="countryId" items={countries} currentId={user.countryId} />
      <AffiliationSelect label="Организация" name="organizationId" items={organizations} currentId={user.organizationId} />
      {error && <p className="error" role="alert">{error}</p>}
      <div className="actions">
        <button type="submit" disabled={saving}>{saving ? 'Сохранение…' : 'Сохранить'}</button>
        <a className="button secondary" href={profileUrl}>Отмена</a>
      </div>
    </form>
  )
}

export function UserEditPage() {
  const id = pageId()
  const profile = useResource(id ? `user/get${id}` : null)
  const countries = useResource(id ? 'country/get' : null)
  const organizations = useResource(id ? 'organization/get' : null)
  const pending = [profile, countries, organizations].find((resource) => resource.error || resource.loading)
  return (
    <section className="panel narrow">
      <h1 className="page-heading">Редактирование профиля</h1>
      {!id && <p className="empty">Участник не выбран.</p>}
      {pending && <ResourceState resource={pending} />}
      {profile.data && countries.data && organizations.data && (
        <ProfileForm key={profile.data.id} user={profile.data} countries={countries.data} organizations={organizations.data} />
      )}
    </section>
  )
}
