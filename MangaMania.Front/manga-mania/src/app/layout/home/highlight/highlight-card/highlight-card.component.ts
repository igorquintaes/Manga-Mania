import { Component, OnInit } from '@angular/core';

import { faBook, faHeart, faTrophy } from '@fortawesome/free-solid-svg-icons'

@Component({
  selector: 'app-highlight-card',
  templateUrl: './highlight-card.component.html',
  styleUrls: ['./highlight-card.component.sass']
})
export class HighlightCardComponent implements OnInit {
  faBook = faBook
  faHeart = faHeart
  faTrophy = faTrophy

  constructor() { }

  ngOnInit(): void {
  }

}
