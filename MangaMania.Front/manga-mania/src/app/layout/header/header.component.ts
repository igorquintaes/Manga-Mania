import { Component, OnInit } from '@angular/core'

import { faFacebookSquare, faInstagramSquare, faTwitterSquare } from '@fortawesome/free-brands-svg-icons'
import { faRssSquare, faBars } from '@fortawesome/free-solid-svg-icons'

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.sass']
})
export class HeaderComponent implements OnInit {
  faFacebookSquare = faFacebookSquare
  faInstagramSquare = faInstagramSquare
  faTwitterSquare = faTwitterSquare
  faRssSquare = faRssSquare
  faBars = faBars

  constructor() { }

  ngOnInit() {
  }

}
