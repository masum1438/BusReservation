import { CommonModule } from "@angular/common";
import { Component, OnInit, signal, computed } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { AdminRouteService } from "../../../../core/services/api.services";
import { DateUtils } from "../../../../core/utils/date-utils";
import { RouteResponseDto, RouteCreateDto } from "../../../../shared/models/models";

@Component({
  selector: 'app-admin-routes',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-routes.html',
  styleUrl: './admin-routes.scss'
})
export class AdminRoutes implements OnInit {
  routes     = signal<RouteResponseDto[]>([]);
  loading    = signal(true);
  showModal  = signal(false);
  saving     = signal(false);
  deleting   = signal<string | null>(null);
  editId     = signal<string | null>(null);
  modalError = signal('');
  search     = '';

  form: RouteCreateDto = { fromCity: '', toCity: '', basePrice: 500, distance: 200 };

  filtered = computed(() =>
    this.routes().filter(r =>
      !this.search ||
      r.fromCity.toLowerCase().includes(this.search.toLowerCase()) ||
      r.toCity.toLowerCase().includes(this.search.toLowerCase())
    )
  );

  fmt = DateUtils;

  constructor(private svc: AdminRouteService) {}
  ngOnInit() { this.load(); }

  load() {
    this.loading.set(true);
    this.svc.getAll().subscribe({ next: r => { this.routes.set(r); this.loading.set(false); }, error: () => this.loading.set(false) });
  }

  openModal(r?: RouteResponseDto) {
    this.modalError.set('');
    this.editId.set(r?.id ?? null);
    this.form = r ? { fromCity: r.fromCity, toCity: r.toCity, basePrice: r.basePrice, distance: r.distance } : { fromCity: '', toCity: '', basePrice: 500, distance: 200 };
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  save() {
    this.saving.set(true); this.modalError.set('');
    const obs = this.editId() ? this.svc.update(this.editId()!, this.form) : this.svc.create(this.form);
    obs.subscribe({ next: () => { this.saving.set(false); this.closeModal(); this.load(); }, error: (e) => { this.saving.set(false); this.modalError.set(e?.error?.error || 'Save failed.'); } });
  }

  deleteRoute(id: string) {
    if (!confirm('Delete this route?')) return;
    this.deleting.set(id);
    this.svc.delete(id).subscribe({ next: () => { this.deleting.set(null); this.load(); }, error: () => this.deleting.set(null) });
  }
}
