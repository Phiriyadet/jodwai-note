const SATURATION = 65;
const LIGHTNESS = 55;

/**
 * Generates a deterministic HSL color from a note id.
 *
 * The same id always produces the same color.
 */
export function getNodeColor(noteId: string): string {
  const hue = getNodeHue(noteId);

  return `hsl(${hue}, ${SATURATION}%, ${LIGHTNESS}%)`;
}

/**
 * Generates a deterministic hue in the range [0, 359].
 */
export function getNodeHue(noteId: string): number {
  let hash = 2166136261;

  for (let i = 0; i < noteId.length; i++) {
    hash ^= noteId.charCodeAt(i);
    hash = Math.imul(hash, 16777619);
  }

  return (hash >>> 0) % 360;
}
