export interface Trip {
  id: string;
  destination: string;
  departureDate: string; // ISO string (DateTime)
  price: number;
  isAvailable: boolean;
}
