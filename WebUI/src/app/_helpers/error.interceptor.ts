import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { AuthService } from '../auth.service';

@Injectable({
  providedIn: 'root'
})
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(private authService:AuthService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        
       // this.message.clear();
        return next.handle(request).pipe(catchError((err:any) => {
            console.log(err.status)
            return throwError(() => err);
        }))
    }
}
 
/*             let reader = new FileReader();
            reader.onload = event => {
                console.log(event)
          
            }
           // reader.readAsArrayBuffer(err);
            
            if ([401, 403].includes(err.status) && this.authService.userValue) {
                // auto logout if 401 Unauthorized or 403 Forbidden response returned from api
                console.log(err);
                this.authService.logout();
            }
//console.log(err)
if(err.status===0){
  //  this.message.add("no network");
    return throwError(() => "no network");
}else{
                const error = err.error.message || err.statusText;
            if(error){
               
           
                          
            }
return throwError(() => error);
}

             */
        

