import { Component, ViewEncapsulation } from '@angular/core';

@Component({
    selector: 'blog-portal',
    styleUrls: [
        './blog-portal.component.scss'
    ],
    templateUrl: './blog-portal.component.html',
    encapsulation: ViewEncapsulation.None
})
export class BlogPortalComponent {
  paceOptions = {
    ajax: true,
    document: true,
    eventLag: true,
    elements: {
      selectors: ['.my-page']
    }
  };
}
