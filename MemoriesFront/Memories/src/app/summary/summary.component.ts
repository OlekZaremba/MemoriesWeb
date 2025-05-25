import { Component, OnInit } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { jsPDF } from 'jspdf';
import { FormsModule } from '@angular/forms';
import { NgForOf, NgIf } from '@angular/common';

@Component({
  selector: 'app-summary',
  standalone: true,
  imports: [FormsModule, NgForOf, NgIf],
  templateUrl: './summary.component.html',
  styleUrl: './summary.component.css'
})
export class SummaryComponent implements OnInit {
  pdfUrl: SafeResourceUrl = '';

  constructor(private sanitizer: DomSanitizer) {}

  ngOnInit() {
    const doc = new jsPDF();

    doc.setFontSize(18);
    doc.text('Podsumowanie semestru', 20, 20);

    doc.setFontSize(12);
    doc.text('Tu bedzie podsumowanie ocen, frekwencji i aktywnosci.', 20, 40);
    doc.text('Dane zostana wygenerowane automatycznie w przyszlosci.', 20, 50);

    // Zamień PDF na blob URL i osadź
    const blob = doc.output('blob');
    const blobUrl = URL.createObjectURL(blob);
    this.pdfUrl = this.sanitizer.bypassSecurityTrustResourceUrl(blobUrl);
  }
}
