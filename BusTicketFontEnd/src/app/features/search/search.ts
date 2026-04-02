// import { Component } from '@angular/core';

// @Component({
//   selector: 'app-search',
//   imports: [],
//   templateUrl: './search.html',
//   styleUrl: './search.scss',
// })
// export class Search {}

import { Component, signal, OnInit, computed } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SearchService } from '../../core/services/api.services';
import { AvailableBusDto } from '../../shared/models/models';
import { DateUtils } from '../../core/utils/date-utils';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './search.html',
  styleUrl: './search.scss'
})
export class Search implements OnInit {
  from     = '';
  to       = '';
  date     = '';
  today    = new Date().toISOString().split('T')[0];
  buses    = signal<AvailableBusDto[]>([]);
  loading  = signal(false);
  searched = signal(false);
  totalSeats = computed(() => this.buses().reduce((s, b) => s + b.seatsLeft, 0));

  // Expose DateUtils to template
  fmt = DateUtils;

  constructor(private route: ActivatedRoute, private router: Router, private svc: SearchService) {}

  ngOnInit() {
    this.route.queryParams.subscribe(p => {
      this.from = p['from'] || '';
      this.to   = p['to'] || '';
      this.date = p['date'] || this.today;
      if (this.from && this.to) this.search();
    });
  }

  search() {
    if (!this.from || !this.to || !this.date) return;
    this.loading.set(true); this.searched.set(false);
    this.svc.searchBuses(this.from, this.to, this.date).subscribe({
      next: r => { this.buses.set(r); this.loading.set(false); this.searched.set(true); },
      error: () => { this.loading.set(false); this.searched.set(true); }
    });
  }

  book(bus: AvailableBusDto) { this.router.navigate(['/booking', bus.busScheduleId]); }
}

