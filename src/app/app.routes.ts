import { Routes } from '@angular/router';
import { homedir } from 'node:os';
import { HomeComponent } from './component/home/home.component';
import { HistoryComponent } from './component/history/history.component';
import { SetComponent } from './component/set/set.component';
import { CreatorComponent } from './component/creator/creator.component';

export const routes: Routes = [
    {
        path: '',
        component: HomeComponent,
        title: "Home Page"
    },
    {
        path:'history',
        component: HistoryComponent,
        title: "History Page"
    },
    {
        path:'set',
        component: SetComponent,
        title: "List"
    },
    {
        path:'creator',
        component: CreatorComponent,
        title: "Creator"
    }
];
