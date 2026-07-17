import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { AuthService } from '../../core/auth.service';
import { extractErrorMessage } from '../../core/error-utils';
import { I18nService } from '../../core/i18n.service';
import { TickerService } from '../../core/ticker.service';
import { ForumPostInfo } from '../../core/models';

@Component({
  selector: 'app-forum',
  imports: [FormsModule, RouterLink],
  templateUrl: './forum.html',
  styleUrl: './forum.css',
})
export class ForumPage {
  readonly i18n = inject(I18nService);
  readonly auth = inject(AuthService);
  readonly ticker = inject(TickerService);
  private readonly api = inject(ApiService);

  readonly posts = signal<ForumPostInfo[]>([]);
  readonly open = signal<ReadonlySet<number>>(new Set());
  readonly drafts: Record<number, string> = {};

  postTitle = '';
  postBody = '';
  readonly error = signal('');

  constructor() {
    this.load();
  }

  private load() {
    this.api.getForumPosts().subscribe((posts) => this.posts.set(posts));
  }

  toggle(postId: number) {
    const next = new Set(this.open());
    if (next.has(postId)) next.delete(postId);
    else next.add(postId);
    this.open.set(next);
  }

  addPost() {
    this.error.set('');
    if (!this.auth.isLoggedIn()) {
      this.error.set(this.i18n.t().signInToPost);
      return;
    }
    if (!this.postTitle.trim() || !this.postBody.trim()) {
      this.error.set(this.i18n.t().fillAll);
      return;
    }
    this.api.addForumPost(this.postTitle, this.postBody).subscribe({
      next: () => {
        this.postTitle = '';
        this.postBody = '';
        this.load();
      },
      error: (err) => this.error.set(extractErrorMessage(err, 'Error')),
    });
  }

  addComment(postId: number) {
    const draft = (this.drafts[postId] ?? '').trim();
    if (!draft) return;
    if (!this.auth.isLoggedIn()) {
      this.error.set(this.i18n.t().signInToPost);
      return;
    }
    this.api.addForumComment(postId, draft).subscribe(() => {
      this.drafts[postId] = '';
      this.load();
    });
  }

  commentLabel(post: ForumPostInfo): string {
    const t = this.i18n.t();
    if (this.open().has(post.id)) return t.hideReplies;
    return `${post.comments.length} ${t.replies}`;
  }
}
