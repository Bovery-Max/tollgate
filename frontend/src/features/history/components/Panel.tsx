// The one frame every dashboard block sits in: an opaque ruled card with a caption bar, so secondary text is never
// placed directly on the silk background.
import type { ReactNode } from 'react'

interface PanelProps {
  id: string
  title: string
  /** Small mono caption on the right of the caption bar. */
  aside?: string
  children: ReactNode
}

export function Panel({ id, title, aside, children }: PanelProps) {
  return (
    <section aria-labelledby={id} className="border border-border bg-card">
      <div className="flex flex-wrap items-baseline justify-between gap-x-4 border-b border-border px-4 py-2">
        <h2 id={id} className="text-xl">
          {title}
        </h2>
        {aside && <p className="label-caps text-muted-foreground">{aside}</p>}
      </div>
      {children}
    </section>
  )
}
