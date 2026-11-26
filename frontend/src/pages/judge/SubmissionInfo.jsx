export function LanguageOptions() {
  return ['C++ 14 (64x)', 'Java 11', 'Python 3.10'].map(language =>
    <option key={language} value={language}>{language}</option>
  )
}

const rejected = new Set(['Wrong Answer', 'Time Limit', 'Memory Limit', 'Runtime Error', 'Presentation Error'])

export function Verdict({ title }) {
  let tone = ''
  if (title === 'Accepted') tone = 'judge-accepted'
  else if (title === 'Compile Error') tone = 'judge-compile'
  else if (rejected.has(title)) tone = 'judge-rejected'
  return <span className={`judge-verdict ${tone}`}>{title || '—'}</span>
}

const formatDate = new Intl.DateTimeFormat('ru-RU', { dateStyle: 'short', timeStyle: 'medium', timeZone: 'Asia/Bishkek' })

export function SubmittedAt({ value }) {
  if (!value) return '—'
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return '—'
  return <time dateTime={value} title="Время Бишкека">{formatDate.format(date)}</time>
}
