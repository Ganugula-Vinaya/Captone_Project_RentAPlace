export interface Review {
  id: number;
  propertyId: number;
  renterId: string;
  renterName?: string;   
  rating: number;
  comment?: string;
  createdAt?: string;
}
