import { User } from "./user";

export class UserParams{
    pageNumber=1;
    pageSize=5;
    minAge=18;
    maxAge=99;
    gender!: string;
    orderBy: string ='lastActive';

    constructor(user:User|null) {
        this.gender=user!.gender == 'male'?'female':'male';
    }
}