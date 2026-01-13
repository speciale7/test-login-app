export interface Busta {
  id: number;
  dataRiferimento: string;
  dataChiusura?: string | null;
  dataRitiro?: string | null;
  sigillo?: string | null;
  totale: number;
  note?: string | null;
  userChiusura?: string | null;
  userRitiro?: string | null;
  createdAt: string;
  updatedAt?: string | null;
  userId: number;
}

export interface CreateBustaDto {
  dataRiferimento: string;
  dataChiusura?: string | null;
  dataRitiro?: string | null;
  sigillo?: string | null;
  totale: number;
  note?: string | null;
  userChiusura?: string | null;
  userRitiro?: string | null;
}

export interface UpdateBustaDto {
  dataRiferimento?: string;
  dataChiusura?: string | null;
  dataRitiro?: string | null;
  sigillo?: string | null;
  totale?: number;
  note?: string | null;
  userChiusura?: string | null;
  userRitiro?: string | null;
}
