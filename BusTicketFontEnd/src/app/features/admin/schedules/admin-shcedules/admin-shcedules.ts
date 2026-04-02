import { CommonModule } from "@angular/common";
import { Component, OnInit, signal, computed } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { AdminScheduleService, AdminBusService, AdminRouteService } from "../../../../core/services/api.services";
import { DateUtils } from "../../../../core/utils/date-utils";
import { ScheduleResponseDto, BusResponseDto, RouteResponseDto, ScheduleCreateDto } from "../../../../shared/models/models";

@Component({
  selector: 'app-admin-schedules',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-shcedules.html',
  styleUrl: './admin-shcedules.scss'
})
export class AdminSchedules implements OnInit {
  schedules  = signal<ScheduleResponseDto[]>([]);
  buses      = signal<BusResponseDto[]>([]);
  routes     = signal<RouteResponseDto[]>([]);
  loading    = signal(true);
  showModal  = signal(false);
  saving     = signal(false);
  deleting   = signal<string | null>(null);
  editId     = signal<string | null>(null);
  modalError = signal('');
  searchText = '';
  filterDate = '';
  filterBusId = '';

  form: ScheduleCreateDto = { busId: '', routeId: '', departureTime: '', arrivalTime: '', price: 0 };

  filtered = computed(() =>
    this.schedules().filter(s => {
      const matchText = !this.searchText ||
        s.busName.toLowerCase().includes(this.searchText.toLowerCase()) ||
        s.fromCity.toLowerCase().includes(this.searchText.toLowerCase()) ||
        s.toCity.toLowerCase().includes(this.searchText.toLowerCase()) ||
        s.companyName.toLowerCase().includes(this.searchText.toLowerCase());
      const matchDate = !this.filterDate || s.departureTime.startsWith(this.filterDate);
      return matchText && matchDate;
    })
  );

  selectedBus  = computed(() => this.buses().find(b => b.id === this.form.busId) ?? null);
  selectedRoute = computed(() => this.routes().find(r => r.id === this.form.routeId) ?? null);

  fmt = DateUtils;

  constructor(
    private scheduleSvc: AdminScheduleService,
    private busSvc: AdminBusService,
    private routeSvc: AdminRouteService
  ) {}

  ngOnInit() {
    this.load();
    this.busSvc.getAll().subscribe(b => this.buses.set(b));
    this.routeSvc.getAll().subscribe(r => this.routes.set(r));
  }

  load() {
    this.loading.set(true);
    this.scheduleSvc.getAll().subscribe({ next: s => { this.schedules.set(s); this.loading.set(false); }, error: () => this.loading.set(false) });
  }

  openModal() {
    this.modalError.set('');
    this.editId.set(null);
    this.form = { busId: '', routeId: '', departureTime: '', arrivalTime: '', price: 0 };
    this.showModal.set(true);
  }

  openEditModal(s: ScheduleResponseDto) {
    this.modalError.set('');
    this.editId.set(s.id);
    // Pre-fill by matching names (since schedule DTO doesn't have IDs back)
    const matchedBus   = this.buses().find(b => b.busName === s.busName && b.companyName === s.companyName);
    const matchedRoute = this.routes().find(r => r.fromCity === s.fromCity && r.toCity === s.toCity);
    this.form = {
      busId:         matchedBus?.id ?? '',
      routeId:       matchedRoute?.id ?? '',
      departureTime: DateUtils.toInputValue(s.departureTime),
      arrivalTime:   DateUtils.toInputValue(s.arrivalTime),
      price:         s.price
    };
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  onBusChange() { /* reactive via computed */ }
  onRouteChange() {
    const r = this.selectedRoute();
    if (r && !this.form.price) this.form.price = r.basePrice;
  }

  // save() {
  //   this.saving.set(true); this.modalError.set('');
  //   const obs = this.editId() ? this.scheduleSvc.update(this.editId()!, this.form) : this.scheduleSvc.create(this.form);
  //   obs.subscribe({
  //     next: () => { this.saving.set(false); this.closeModal(); this.load(); },
  //     error: (e) => { this.saving.set(false); this.modalError.set(e?.error?.error || 'Save failed. Check all fields.'); }
  //   });
  // }
  save() {
  this.saving.set(true);
  this.modalError.set('');

  // 🔥 Convert datetime-local → ISO UTC
  const dto: ScheduleCreateDto = {
    busId: this.form.busId,
    routeId: this.form.routeId,
    departureTime: DateUtils.fromInputValue(this.form.departureTime),
    arrivalTime: DateUtils.fromInputValue(this.form.arrivalTime),
    price: this.form.price
  };

  const obs = this.editId()
    ? this.scheduleSvc.update(this.editId()!, dto)
    : this.scheduleSvc.create(dto);

  obs.subscribe({
    next: () => {
      this.saving.set(false);
      this.closeModal();
      this.load();
    },
    error: (e) => {
      this.saving.set(false);

      console.error("Schedule Save Error:", e);

      this.modalError.set(
        e?.error?.error ||
        e?.error?.title ||
        'Save failed. Check all fields.'
      );
    }
  });
}

  deleteSchedule(id: string) {
    if (!confirm('Delete this schedule? All associated seats will be removed.')) return;
    this.deleting.set(id);
    this.scheduleSvc.delete(id).subscribe({ next: () => { this.deleting.set(null); this.load(); }, error: () => this.deleting.set(null) });
  }

  getDuration(dep: string, arr: string): string {
    const m = Math.round((new Date(arr).getTime() - new Date(dep).getTime()) / 60000);
    if (m <= 0) return '—';
    return m >= 60 ? `${Math.floor(m / 60)}h ${m % 60}m` : `${m}m`;
  }
}