export type CharityCase = {
    id: number;
    title: string;
    description: string;
    amountRequested: number;
    amountCollected: number;
    category: string;
    urgencyLevel: string;
    requestDate: Date;
    endDate: Date;
    isActive: boolean;
    imageUrl: string;
    beneficiaryName: string;

}