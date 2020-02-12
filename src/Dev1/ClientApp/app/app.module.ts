import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import './vendor';
import { Dev1SharedModule } from 'app/shared/shared.module';
import { Dev1CoreModule } from 'app/core/core.module';
import { Dev1AppRoutingModule } from './app-routing.module';
import { Dev1HomeModule } from './home/home.module';
import { Dev1EntityModule } from './entities/entity.module';
// jhipster-needle-angular-add-module-import JHipster will add new module here
import { MainComponent } from './layouts/main/main.component';
import { NavbarComponent } from './layouts/navbar/navbar.component';
import { FooterComponent } from './layouts/footer/footer.component';
import { PageRibbonComponent } from './layouts/profiles/page-ribbon.component';
import { ErrorComponent } from './layouts/error/error.component';

@NgModule({
  imports: [
    BrowserModule,
    Dev1SharedModule,
    Dev1CoreModule,
    Dev1HomeModule,
    // jhipster-needle-angular-add-module JHipster will add new module here
    Dev1EntityModule,
    Dev1AppRoutingModule
  ],
  declarations: [MainComponent, NavbarComponent, ErrorComponent, PageRibbonComponent, FooterComponent],
  bootstrap: [MainComponent]
})
export class Dev1AppModule {}
