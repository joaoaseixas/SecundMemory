export interface TreeNode<T> {
  item: T;
  children: TreeNode<T>[];
}

/** Mirrors Application.Features.Blocks.Queries.GetBlockTree.GetBlockTreeHandler.Build. */
export function buildTree<T>(items: T[], getId: (item: T) => string, getParentId: (item: T) => string | null): TreeNode<T>[] {
  const byParent = new Map<string | null, T[]>();
  for (const item of items) {
    const parentId = getParentId(item);
    byParent.set(parentId, [...(byParent.get(parentId) ?? []), item]);
  }

  const build = (parentId: string | null): TreeNode<T>[] =>
    (byParent.get(parentId) ?? []).map((item) => ({ item, children: build(getId(item)) }));

  return build(null);
}

/**
 * Fractional ranking for drag-and-drop reordering: the new order is the
 * midpoint between its neighbors, so nothing else on the page needs to be
 * rewritten when a block is dropped between two existing ones.
 */
export function computeOrderBetween(before: number | null, after: number | null): number {
  if (before === null && after === null) return 1;
  if (before === null) return after! - 1;
  if (after === null) return before + 1;
  return (before + after) / 2;
}
