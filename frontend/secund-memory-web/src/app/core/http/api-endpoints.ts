/** Mirrors backend/src/SecundMemory.Api/Controllers route attributes — one place to update if a route ever moves. */
export const ApiEndpoints = {
  auth: {
    register: 'auth/register',
    login: 'auth/login',
    refresh: 'auth/refresh',
  },
  pages: {
    base: 'pages',
    byId: (id: string) => `pages/${id}`,
    byTitle: 'pages/by-title',
    backlinks: (id: string) => `pages/${id}/backlinks`,
    title: (id: string) => `pages/${id}/title`,
  },
  blocks: {
    base: 'blocks',
    byId: (id: string) => `blocks/${id}`,
    content: (id: string) => `blocks/${id}/content`,
    move: (id: string) => `blocks/${id}/move`,
    collapse: (id: string) => `blocks/${id}/collapse`,
    type: (id: string) => `blocks/${id}/type`,
    treeForPage: (pageId: string) => `pages/${pageId}/blocks`,
  },
  journals: {
    today: 'journals/today',
    byDate: (date: string) => `journals/${date}`,
  },
  tags: {
    base: 'tags',
    applyToBlock: (tagId: string, blockId: string) => `tags/${tagId}/blocks/${blockId}`,
  },
  search: 'search',
  graph: 'graph',
  attachments: 'attachments',
} as const;
