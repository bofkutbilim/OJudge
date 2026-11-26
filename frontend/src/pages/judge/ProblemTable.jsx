import { RemoveButton } from '../../forms.jsx'

function difficultyLevel(value) {
  if (value == null) return 'Undefined'
  if (value < 33) return 'Easy'
  if (value < 66) return 'Medium'
  return 'Hard'
}

export function ProblemTable({ problems, onRemove }) {
  if (!problems.length) return <p className="empty">Задач не найдено.</p>

  return (
    <div className="table-wrap">
      <table>
        <thead>
          <tr>
            <th scope="col">№</th><th scope="col">Название</th><th scope="col">Темы</th>
            <th scope="col">Сложность</th><th scope="col">Решили</th>
            {onRemove && <th scope="col" aria-label="Действия" />}
          </tr>
        </thead>
        <tbody>
          {problems.map(problem => {
            const level = difficultyLevel(problem.difficulty)
            const progress = problem.isSolved ? 'solved' : problem.isSubmitted ? 'attempted' : ''
            return (
              <tr key={problem.id} className={progress && `judge-${progress}`}>
                <td className="judge-id">
                  <span title={progress === 'solved' ? 'Задача решена' : progress === 'attempted' ? 'Есть попытка решения' : undefined}>{problem.id}</span>
                </td>
                <td><a href={`/problem.html?id=${problem.id}`}>{problem.title}</a></td>
                <td>{problem.topicShortTitle?.join(', ') || '—'}</td>
                <td><span className={`judge-level level-${level.toLowerCase()}`}>{level}</span></td>
                <td>{problem.solved ?? 0}</td>
                {onRemove && <td><div className="table-actions">
                  <a className="icon-button" href={`/problems/edit.html?id=${problem.id}`} aria-label={`Изменить задачу «${problem.title}»`} title="Изменить задачу">✎</a>
                  <RemoveButton label={`Удалить задачу «${problem.title}»`} question={`Удалить задачу «${problem.title}»?`} onRemove={() => onRemove(problem.id)} />
                </div></td>}
              </tr>
            )
          })}
        </tbody>
      </table>
    </div>
  )
}
