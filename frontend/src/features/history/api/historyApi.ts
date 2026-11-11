// The only place the history page talks HTTP: the two read-only endpoints, each reduced to data or a HistoryFailure.
import { api } from '@/lib/api/client'
import type { HistoryResponse, StatsResponse } from '@/lib/api/types'
import { historyFailureFromResponse, type HistoryFailure } from '../logic/historyFailure'

export type Outcome<T> = { ok: true; data: T } | { ok: false; failure: HistoryFailure }

async function settle<T>(request: Promise<{ data?: T; response: Response }>): Promise<Outcome<T>> {
  try {
    const { data, response } = await request
    if (data) return { ok: true, data }
    return { ok: false, failure: historyFailureFromResponse(response.status, response.headers.get('Retry-After')) }
  } catch {
    return { ok: false, failure: { kind: 'offline' } } // fetch throws only when the server cannot be reached at all
  }
}

export const fetchStats = (): Promise<Outcome<StatsResponse>> => settle(api.GET('/api/stats'))

export const fetchHistory = (limit: number): Promise<Outcome<HistoryResponse>> =>
  settle(api.GET('/api/history', { params: { query: { limit } } }))
