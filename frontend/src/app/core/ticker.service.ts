import { Injectable, signal } from '@angular/core';
import { Dict } from './i18n.service';

export interface TimeLeft {
  label: string;
  urgent: boolean;
  ended: boolean;
}

@Injectable({ providedIn: 'root' })
export class TickerService {
  /** Ceas partajat, actualizat la fiecare secunda — alimenteaza countdown-urile. */
  readonly now = signal(Date.now());

  constructor() {
    setInterval(() => this.now.set(Date.now()), 1000);
  }

  timeLeft(endDate: string, t: Dict): TimeLeft {
    const diff = new Date(endDate).getTime() - this.now();
    if (diff <= 0) return { label: t.ended, urgent: false, ended: true };
    const s = Math.floor(diff / 1000);
    const dd = Math.floor(s / 86400);
    const hh = Math.floor((s % 86400) / 3600);
    const mm = Math.floor((s % 3600) / 60);
    const ss = s % 60;
    let label: string;
    if (dd > 0) label = `${dd}d ${hh}h`;
    else if (hh > 0) label = `${hh}h ${mm}m`;
    else label = `${mm}m ${String(ss).padStart(2, '0')}s`;
    return { label, urgent: diff < 3600000, ended: false };
  }

  ago(date: string, t: Dict): string {
    const diff = this.now() - new Date(date).getTime();
    const mm = Math.floor(diff / 60000);
    if (mm < 60) return `${Math.max(mm, 0)} ${t.minAgo}`;
    const hh = Math.floor(mm / 60);
    if (hh < 24) return `${hh}${t.hAgo}`;
    return `${Math.floor(hh / 24)}${t.dAgo}`;
  }
}
