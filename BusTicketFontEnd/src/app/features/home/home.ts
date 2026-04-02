// import { Component } from '@angular/core';

// @Component({
//   selector: 'app-home',
//   imports: [],
//   templateUrl: './home.html',
//   styleUrl: './home.scss',
// })
// export class Home {}

import { Component, OnInit, signal, computed } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SearchService } from '../../core/services/api.services';
import { AvailableBusDto } from '../../shared/models/models';
import { DateUtils } from '../../core/utils/date-utils';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './home.html',
  styleUrl: './home.scss'
  
})
export class Home implements OnInit {

  // ── Search form state ─────────────────────────────────────────
  from  = '';
  to    = '';
  date  = '';
  today = new Date().toISOString().split('T')[0];   // local YYYY-MM-DD

  // ── Live departures ───────────────────────────────────────────
  liveDepartures  = signal<AvailableBusDto[]>([]);
  searchingLive   = signal(true);

  // ── Expose DateUtils to template ─────────────────────────────
  fmt = DateUtils;

  // ── Suggested city list ───────────────────────────────────────
  cities = [
    'Dhaka', 'Chittagong', 'Sylhet', 'Rajshahi', 'Khulna',
    'Barisal', 'Rangpur', 'Mymensingh', "Cox's Bazar",
    'Comilla', 'Jessore', 'Bogra', 'Tangail', 'Gazipur'
  ];

  // ── Date shortcuts ────────────────────────────────────────────
  dateShortcuts = this.buildDateShortcuts();

  // ── Feature pills ─────────────────────────────────────────────
  featurePills = [
    { icon: 'bi-lightning-charge-fill', text: 'Instant booking'  },
    { icon: 'bi-seat-fill',             text: 'Live seat map'     },
    { icon: 'bi-shield-fill-check',     text: 'Secure checkout'   },
    { icon: 'bi-arrow-repeat',          text: 'Free cancellation' },
  ];

  // ── Live stats ────────────────────────────────────────────────
  liveStats = [
    { value: '200+',  label: 'Routes nationwide'  },
    { value: '50K+',  label: 'Happy travelers'     },
    { value: '<60s',  label: 'Average booking time'},
    { value: '24/7',  label: 'Live support'         },
  ];

  // ── How it works ──────────────────────────────────────────────
  howItWorks = [
    { num: '01', icon: 'bi-search',              color: '#0d6efd', bg: 'rgba(13,110,253,.08)', title: 'Search Your Route',      desc: 'Enter departure city, destination and travel date. We search all available buses in real time.' },
    { num: '02', icon: 'bi-grid-3x3-gap-fill',   color: '#10b981', bg: 'rgba(16,185,129,.08)', title: 'Pick Your Seat',          desc: 'See a live seat map. Green = available, red = taken. Click to reserve — it\'s held for 10 minutes.' },
    { num: '03', icon: 'bi-ticket-perforated-fill', color: '#f59e0b', bg: 'rgba(245,158,11,.08)', title: 'Get Your Ticket',      desc: 'Confirm details, submit, and receive your ticket number instantly. No payment gateway needed.' },
  ];

  // ── Features grid ─────────────────────────────────────────────
  features = [
    { icon: 'bi-lightning-charge-fill', color: '#0d6efd', bg: 'rgba(13,110,253,.08)',  title: 'Real-Time Seat Map',     desc: 'Interactive live seat plan shows exactly which seats are free. Updated instantly when someone books.' },
    { icon: 'bi-lock-fill',             color: '#10b981', bg: 'rgba(16,185,129,.08)',  title: 'Seat Lock (10 min)',      desc: 'Found your seat? We lock it for 10 minutes while you complete your booking — no one can steal it.' },
    { icon: 'bi-search-heart-fill',     color: '#8b5cf6', bg: 'rgba(139,92,246,.08)',  title: 'Smart Route Search',     desc: 'Search by city name and date. Results are sorted by departure time with seat counts shown.' },
    { icon: 'bi-ticket-perforated-fill',color: '#f59e0b', bg: 'rgba(245,158,11,.08)', title: 'Unique Ticket Numbers',  desc: 'Every booking gets a unique TKT-YYYYMMDD-XXXXXXXX reference for easy tracking and verification.' },
    { icon: 'bi-arrow-counterclockwise',color: '#ef4444', bg: 'rgba(239,68,68,.08)',  title: 'Free Cancellation',      desc: 'Cancel up to 2 hours before departure at no cost. Seat is automatically released back to the pool.' },
    { icon: 'bi-shield-fill-check',     color: '#0ea5e9', bg: 'rgba(14,165,233,.08)', title: 'JWT-Secured Account',    desc: 'Your account is protected with JWT access tokens and 7-day refresh tokens with automatic rotation.' },
  ];

  // ── Popular routes ────────────────────────────────────────────
  popularRoutes = [
    { from: 'Dhaka',      to: 'Chittagong',   price: 550,  distance: 265,  gradient: 'linear-gradient(135deg,#0d6efd,#6366f1)' },
    { from: 'Dhaka',      to: 'Sylhet',        price: 480,  distance: 244,  gradient: 'linear-gradient(135deg,#10b981,#0ea5e9)' },
    { from: 'Dhaka',      to: 'Rajshahi',      price: 420,  distance: 252,  gradient: 'linear-gradient(135deg,#f59e0b,#ef4444)' },
    { from: 'Dhaka',      to: "Cox's Bazar",   price: 750,  distance: 395,  gradient: 'linear-gradient(135deg,#8b5cf6,#ec4899)' },
    { from: 'Chittagong', to: "Cox's Bazar",   price: 280,  distance: 153,  gradient: 'linear-gradient(135deg,#0ea5e9,#10b981)' },
    { from: 'Dhaka',      to: 'Khulna',        price: 500,  distance: 275,  gradient: 'linear-gradient(135deg,#f59e0b,#10b981)' },
    { from: 'Dhaka',      to: 'Barisal',       price: 380,  distance: 190,  gradient: 'linear-gradient(135deg,#6366f1,#ec4899)' },
    { from: 'Dhaka',      to: 'Rangpur',       price: 440,  distance: 300,  gradient: 'linear-gradient(135deg,#ef4444,#f59e0b)' },
  ];

  constructor(private router: Router, private searchSvc: SearchService) {
    this.date = this.today;
  }

  ngOnInit() {
    // Load today's Dhaka departures for the live feed section
    //this.loadLiveDepartures();
  }

  // ── Live departures loader ─────────────────────────────────────
  private loadLiveDepartures() {
    this.searchingLive.set(true);
    this.searchSvc.searchBuses('Dhaka', '', this.today).subscribe({
      next: buses => {
        // Show only upcoming departures — compare UTC times
        const now = Date.now();
        const upcoming = buses
          .filter(b => {
            const depDate = DateUtils.toDate(b.startTime);
            return depDate && depDate.getTime() > now;
          })
          .slice(0, 6);
        this.liveDepartures.set(upcoming);
        this.searchingLive.set(false);
      },
      error: () => {
        // Silently fail — live feed is a bonus feature, not critical
        this.liveDepartures.set([]);
        this.searchingLive.set(false);
      }
    });
  }

  // ── Format the selected journey date for display ───────────────
  formatSelectedDate(): string {
    if (!this.date) return '';
    // The date input gives us a local "YYYY-MM-DD" string.
    // We append T00:00:00 and let DateUtils normalise to UTC for display.
    return DateUtils.dateLong(this.date + 'T00:00:00');
  }

  // ── Today's label in readable form ────────────────────────────
  get todayLabel(): string {
    return DateUtils.dateLong(this.today + 'T00:00:00');
  }

  // ── Live clock signals for the datetime transparency banner ───
  localTime(): string {
    const d = new Date();
    return d.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: false });
  }

  utcTime(): string {
    const d = new Date();
    return d.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: false, timeZone: 'UTC' }) + ' UTC';
  }

  // ── Extract destination from a bus DTO ────────────────────────
  // (The search API doesn't return toCity on each bus — we infer from the
  //  popular routes list or fall back to "Destination")
  getDestination(bus: AvailableBusDto): string {
    // Bus name often contains the destination, but that's unreliable.
    // In production, the backend search endpoint should return the route toCity.
    // For now, we return a friendly placeholder.
    return 'Destination';
  }

  // ── Date shortcut buttons ──────────────────────────────────────
  private buildDateShortcuts(): { label: string; value: string }[] {
    const shortcuts = [];
    for (let i = 0; i < 4; i++) {
      const d = new Date();
      d.setDate(d.getDate() + i);
      const value = d.toISOString().split('T')[0];
      const label = i === 0 ? 'Today'
                  : i === 1 ? 'Tomorrow'
                  : d.toLocaleDateString('en-GB', { weekday: 'short', day: '2-digit', month: 'short' });
      shortcuts.push({ label, value });
    }
    return shortcuts;
  }

  // ── Swap from/to cities ────────────────────────────────────────
  swapCities() {
    [this.from, this.to] = [this.to, this.from];
  }

  // ── Navigate to search ─────────────────────────────────────────
  search() {
    if (this.from && this.to && this.date) {
      this.router.navigate(['/search'], {
        queryParams: { from: this.from, to: this.to, date: this.date }
      });
    }
  }

  quickSearch(from: string, to: string) {
    this.router.navigate(['/search'], {
      queryParams: { from, to, date: this.today }
    });
  }

  bookNow(bus: AvailableBusDto) {
    this.router.navigate(['/booking', bus.busScheduleId]);
  }
}
