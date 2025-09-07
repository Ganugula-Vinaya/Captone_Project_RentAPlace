export interface ReservationCreateDto {
  propertyId: number;
  checkIn: string;  // yyyy-mm-dd
  checkOut: string; // yyyy-mm-dd
  guests: number;
}
