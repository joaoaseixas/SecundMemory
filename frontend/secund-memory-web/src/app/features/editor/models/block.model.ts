export type BlockType = 'Text' | 'Heading1' | 'Heading2' | 'Heading3' | 'Quote' | 'Code';

export interface Block {
  id: string;
  pageId: string;
  parentBlockId: string | null;
  content: string;
  type: BlockType;
  order: number;
  isCollapsed: boolean;
  tagIds: string[];
  createdAtUtc: string;
  lastModifiedAtUtc: string | null;
}

export interface BlockTreeNode {
  block: Block;
  children: BlockTreeNode[];
}
