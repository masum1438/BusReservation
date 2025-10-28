export interface Seat {
  seatId: string;
  seatNumber: string;
  row: number;
  status: number; // 0 = Available, 1 = Booked, 2 = Sold
  selected?: boolean;
}
