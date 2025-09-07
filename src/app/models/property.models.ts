export interface PropertyCard {
  id: number;
  ownerId: string;
  title: string;
  city: string;
  country: string;
  pricePerNight: number;
  propertyType: string;
  averageRating: number;
  images: string[];
}

export interface PropertyFull {
  id: number;
  ownerId: string;
  title: string;
  description?: string;
  address?: string;
  city: string;
  state?: string;
  country: string;
  latitude?: number;
  longitude?: number;
  pricePerNight: number;
  propertyType: string;
  maxGuests: number;
  bedrooms: number;
  bathrooms: number;
  isActive: boolean;
  averageRating: number;
  images: { id: number; propertyId: number; imageUrl: string }[];
  propertyAmenities: { propertyId: number; amenityId: number; amenity: { id: number; name: string } }[];
}

export interface SearchQuery {
  city?: string;
  country?: string;
  propertyType?: string;
  featuresCsv?: string;
  checkIn?: string;
  checkOut?: string;
  guests?: number;
  page?: number;
  pageSize?: number;
}

export interface PropertyCreateDto {
  title: string;
  description?: string;
  address?: string;
  city: string;
  state?: string;
  country: string;
  latitude?: number;
  longitude?: number;
  pricePerNight: number;
  propertyType: string;
  maxGuests: number;
  bedrooms: number;
  bathrooms: number;
  amenityIds: number[];
}
