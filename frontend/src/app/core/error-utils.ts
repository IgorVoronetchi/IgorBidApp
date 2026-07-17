/**
 * Extrage un mesaj de eroare afisabil dintr-un raspuns HTTP de eroare.
 * Acopera atat BadRequest("mesaj text") cat si ValidationProblemDetails
 * (400 automat generat de [ApiController] pentru DataAnnotations invalide).
 */
export function extractErrorMessage(err: unknown, fallback: string): string {
  const body = (err as { error?: unknown })?.error;

  if (typeof body === 'string' && body.trim()) return body;

  const problem = body as { errors?: Record<string, string[]>; title?: string } | undefined;
  if (problem?.errors) {
    const firstField = Object.values(problem.errors)[0];
    if (firstField?.length) return firstField[0];
  }
  if (problem?.title) return problem.title;

  return fallback;
}
