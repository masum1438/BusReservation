import { CommonModule } from "@angular/common";
import { Component, OnInit, signal, computed } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { AdminBusService } from "../../../../core/services/api.services";
import { DateUtils } from "../../../../core/utils/date-utils";
import { BusResponseDto, BusCreateDto } from "../../../../shared/models/models";


@Component({
  selector: 'app-admin-buses',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-buses.html',
  styleUrl: './admin-buses.scss'
 
})
export class AdminBuses implements OnInit {
  buses      = signal<BusResponseDto[]>([]);
  loading    = signal(true);
  showModal  = signal(false);
  saving     = signal(false);
  deleting   = signal<string | null>(null);
  editId     = signal<string | null>(null);
  error      = signal('');
  modalError = signal('');
  searchText = '';
  filterType = '';

  form: BusCreateDto = { busNumber: '', companyName: '', busName: '', totalSeats: 40, type: 1 };

  filtered = computed(() =>
    this.buses().filter(b => {
      const matchSearch = !this.searchText ||
        b.busName.toLowerCase().includes(this.searchText.toLowerCase()) ||
        b.companyName.toLowerCase().includes(this.searchText.toLowerCase()) ||
        b.busNumber.toLowerCase().includes(this.searchText.toLowerCase());
      const matchType = !this.filterType || b.type === this.filterType;
      return matchSearch && matchType;
    })
  );

  fmt = DateUtils;

  constructor(private svc: AdminBusService) {}
  ngOnInit() { this.load(); }

  load() {
    this.loading.set(true);
    this.svc.getAll().subscribe({ next: b => { this.buses.set(b); this.loading.set(false); }, error: () => { this.error.set('Failed to load buses.'); this.loading.set(false); } });
  }

  applyFilter() { /* computed handles this reactively */ }

  openModal(bus?: BusResponseDto) {
    this.modalError.set('');
    if (bus) {
      this.editId.set(bus.id);
      this.form = { busNumber: bus.busNumber, companyName: bus.companyName, busName: bus.busName, totalSeats: bus.totalSeats, type: this.typeNum(bus.type) };
    } else {
      this.editId.set(null);
      this.form = { busNumber: '', companyName: '', busName: '', totalSeats: 40, type: 1 };
    }
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  save() {
    this.saving.set(true); this.modalError.set('');
    const obs = this.editId() ? this.svc.update(this.editId()!, this.form) : this.svc.create(this.form);
    obs.subscribe({
      next: () => { this.saving.set(false); this.closeModal(); this.load(); },
      error: (e) => { this.saving.set(false); this.modalError.set(e?.error?.error || 'Save failed.'); }
    });
  }

  deleteBus(id: string) {
    if (!confirm('Delete this bus? This cannot be undone.')) return;
    this.deleting.set(id);
    this.svc.delete(id).subscribe({ next: () => { this.deleting.set(null); this.load(); }, error: () => this.deleting.set(null) });
  }

  typeColor(t: string) { return {AC:'#0d6efd',NonAC:'#64748b',Sleeper:'#6366f1',Luxury:'#f59e0b'}[t] ?? '#64748b'; }
  typeLightBg(t: string) { return {AC:'rgba(13,110,253,.07)',NonAC:'rgba(100,116,139,.07)',Sleeper:'rgba(99,102,241,.07)',Luxury:'rgba(245,158,11,.07)'}[t] ?? 'rgba(0,0,0,.04)'; }
  typeBg(t: string) { return {AC:'linear-gradient(90deg,#0d6efd,#6366f1)',NonAC:'linear-gradient(90deg,#64748b,#94a3b8)',Sleeper:'linear-gradient(90deg,#6366f1,#8b5cf6)',Luxury:'linear-gradient(90deg,#f59e0b,#ef4444)'}[t] ?? '#e2e8f0'; }
  typeLabel(n: number) { return {1:'AC',2:'NonAC',3:'Sleeper',4:'Luxury'}[n] ?? 'AC'; }
  typeNum(t: string) { return ({'AC':1,'NonAC':2,'Sleeper':3,'Luxury':4}[t]) ?? 1; }
}
