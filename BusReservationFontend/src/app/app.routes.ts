import { Routes } from '@angular/router';
import { Search } from './pages/search/search';
import { SeatPlan } from './pages/seat-plan/seat-plan';

export const routes: Routes = [
     { path: '', component: Search},
  { path: 'seat-plan/:busScheduleId', component: SeatPlan },
  { path: '**', redirectTo: '' }
];
