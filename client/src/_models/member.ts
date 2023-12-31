import { Photo } from "./photo"

export interface Member {
    id: number
    userName: string
    photoUrl: string
    age: number
    dateOfBirth: string
    knownAs: string
    created: string
    lastActive: string
    gender: string
    introduction: string
    lookingFor: string
    interstes: any
    city: string
    country: string
    photos: Photo[]
  }
  
