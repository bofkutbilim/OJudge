import { useEffect, useState } from 'react'
import { api, useSession } from './api.js'
import { ResourceState, SessionProvider } from './components.jsx'
import { routes } from './routes.js'
import * as accounts from './pages/AccountPages.jsx'
import * as judge from './pages/JudgePages.jsx'
import * as topics from './pages/TopicPages.jsx'
import './App.css'

const navigation = [
  ['archive', '/archieve.html', 'Архив'],
  ['submissions', '/submissions.html', 'Посылки'],
  ['topics', '/topics.html', 'Темы'],
  ['ranklist', '/ranklist.html', 'Рейтинг'],
]

function NotFoundPage() {
  return (
    <section className="panel narrow not-found">
      <p className="error-code">404</p>
      <h1>Страница не найдена</h1>
      <a className="button" href="/archieve.html">В архив задач</a>
    </section>
  )
}

const views = { ...accounts, ...judge, ...topics, NotFoundPage }

function Header({ route }) {
  const session = useSession()
  const [busy, setBusy] = useState(false)
  const [logoutError, setLogoutError] = useState('')

  async function logout() {
    setBusy(true)
    setLogoutError('')
    try {
      await api('user/logout', { method: 'PUT' })
      const list = navigation.find(([, href]) => href === route.path)
      window.location.assign(list ? list[1] : '/archieve.html')
    } catch (error) {
      setLogoutError(error.message)
      setBusy(false)
      session.reload()
    }
  }

  return (
    <>
      <a href="#main" className="skip-link">К содержимому</a>
      <header className="site-header">
        <div className="header-inner">
          <a className="brand" href="/archieve.html" aria-label="OnlineJudge — архив задач">
            <span className="brand-mark" aria-hidden="true">›_</span>OnlineJudge
          </a>
          <nav className="site-nav" aria-label="Основная навигация">
            {navigation.map(([section, href, label]) => (
              <a key={section} href={href} aria-current={route.section === section
                ? route.path === href ? 'page' : 'location'
                : undefined}>{label}</a>
            ))}
          </nav>
          <div className="session-nav">
            {session.loading ? <span role="status">Загрузка…</span> : session.user ? (
              <>
                <a className="session-profile" href={`/users/profile.html?id=${session.user.userId}`}>
                  {session.user.userNickName}
                </a>
                <button type="button" className="session-button" disabled={busy} onClick={logout}>Выйти</button>
              </>
            ) : !session.error && route.view !== 'LoginPage' && (
              <a className="session-button" href="/login.html">Войти</a>
            )}
          </div>
        </div>
      </header>
      {(session.error || logoutError) && (
        <div className="shell-notice">
          {session.error && <ResourceState resource={session} />}
          {logoutError && <p role="alert" className="error">{logoutError}</p>}
        </div>
      )}
    </>
  )
}

export default function App() {
  const pathname = window.location.pathname
  const path = pathname === '/' || pathname === '/index.html' ? '/archieve.html' : pathname
  const route = routes.find(entry => entry.path === path) || routes.find(entry => entry.view === 'NotFoundPage')
  const Page = views[route.view]
  useEffect(() => { document.title = `${route.title} · OnlineJudge` }, [route])

  return (
    <SessionProvider>
      <Header route={route} />
      <main id="main" className="workspace" tabIndex="-1"><Page /></main>
    </SessionProvider>
  )
}
