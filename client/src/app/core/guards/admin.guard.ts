import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { SnackbarService } from '../services/snackbar.service';
import { AccountService } from '../services/account.service';

export const adminGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const snack  = inject(SnackbarService);
  const accountService = inject(AccountService);
  if(accountService.isAdmin()){
      return true;
  }else{
    snack.error('Nope');
    router.navigateByUrl('./portal');
    return false;
  }
};
