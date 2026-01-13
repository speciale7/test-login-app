// Cassa Intelligente Models
export interface CassaIntelligente {
  id: number;
  idStore: number;
  storeAlias?: string;
  countingDate?: Date;
  securityEnvelopeCode?: string;
  countingAmount?: number;
  countingDifference?: number;
  countingCoins?: number;
  withdrawalDate?: Date;
  countingCourierDate?: Date;
  importDate?: Date;
  countingAt?: Date;
  countingBy?: string;
}

export interface CreateCassaInteligenteDto {
  idStore: number;
  storeAlias?: string;
  countingDate?: Date;
  securityEnvelopeCode?: string;
  countingAmount?: number;
  countingDifference?: number;
  countingCoins?: number;
  withdrawalDate?: Date;
}

export interface UpdateCassaInteligenteDto {
  storeAlias?: string;
  countingDate?: Date;
  securityEnvelopeCode?: string;
  countingAmount?: number;
  countingDifference?: number;
  countingCoins?: number;
  withdrawalDate?: Date;
}

// Fondo Spese Models
export interface FondoSpese {
  id: number;
  idStore: number;
  countingDate?: Date;
  expenseType?: string;
  countingAmount?: number;
  countingCoins?: number;
  invoiceDate?: Date;
  invoiceNumber?: string;
  reasonExpenses?: string;
  countingAt?: Date;
  countingBy?: string;
}

export interface CreateFondoSpeseDto {
  idStore: number;
  countingDate?: Date;
  expenseType?: string;
  countingAmount?: number;
  countingCoins?: number;
  invoiceDate?: Date;
  invoiceNumber?: string;
  reasonExpenses?: string;
}

export interface UpdateFondoSpeseDto {
  countingDate?: Date;
  expenseType?: string;
  countingAmount?: number;
  countingCoins?: number;
  invoiceDate?: Date;
  invoiceNumber?: string;
  reasonExpenses?: string;
}

// Fondo Cassa Models
export interface FondoCassa {
  id: number;
  idStore: number;
  cashCode?: string;
  countingDate?: Date;
  countingAmount?: number;
  countingCoins?: number;
  countingAt?: Date;
  countingBy?: string;
}

export interface CreateFondoCassaDto {
  idStore: number;
  cashCode?: string;
  countingDate?: Date;
  countingAmount?: number;
  countingCoins?: number;
}

export interface UpdateFondoCassaDto {
  cashCode?: string;
  countingDate?: Date;
  countingAmount?: number;
  countingCoins?: number;
}

// Sovv. Monetaria Models
export interface SovvMonetaria {
  id: number;
  idStore: number;
  countingDate?: Date;
  countingAmount?: number;
  countingNote?: string;
  countingCoins?: number;
  countingAt?: Date;
  countingBy?: string;
}

export interface CreateSovvMonetariaDto {
  idStore: number;
  countingDate?: Date;
  countingAmount?: number;
  countingNote?: string;
  countingCoins?: number;
}

export interface UpdateSovvMonetariaDto {
  countingDate?: Date;
  countingAmount?: number;
  countingNote?: string;
  countingCoins?: number;
}
