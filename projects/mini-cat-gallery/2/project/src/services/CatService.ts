
export interface CatImage {
  id: string
  url: string
  width: number
  height: number
}

const ENDPOINT = 'https://api.thecatapi.com/v1/images/search?limit=6'

export async function fetchRandomCats (): Promise<CatImage[]> {
  const res = await fetch(ENDPOINT, {
    headers: { 'Content-Type': 'application/json' }
  })

  if (!res.ok) {
    throw new Error(`Cat API failed to request（HTTP ${res.status}）`)
  }
  return await res.json() as CatImage[]
}
