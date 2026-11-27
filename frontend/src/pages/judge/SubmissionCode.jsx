import { useMemo } from 'react'
import hljs from 'highlight.js/lib/core'
import cpp from 'highlight.js/lib/languages/cpp'
import java from 'highlight.js/lib/languages/java'
import python from 'highlight.js/lib/languages/python'
import 'highlight.js/styles/github-dark.css'

hljs.registerLanguage('cpp', cpp)
hljs.registerLanguage('java', java)
hljs.registerLanguage('python', python)

const languages = new Map([['c++', 'cpp'], ['cpp', 'cpp'], ['java', 'java'], ['python', 'python']])

export default function SubmissionCode({ code, languageTitle }) {
  const language = languages.get(languageTitle.trim().toLowerCase().split(/\s+/, 1)[0])
  const markup = useMemo(() => language ? {
    // highlight.js escapes the source text before adding its own token spans.
    __html: hljs.highlight(code, { language, ignoreIllegals: true }).value,
  } : null, [code, language])

  return <pre className="submission-source" tabIndex={0} aria-label="Исходный код">{
    markup ? <code className="hljs" dangerouslySetInnerHTML={markup} /> : <code>{code}</code>
  }</pre>
}
