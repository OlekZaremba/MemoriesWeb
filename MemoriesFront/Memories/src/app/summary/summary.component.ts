import { Component, OnInit } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { jsPDF } from 'jspdf';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

interface GradeSummaryDTO {
  id: number;
  grade: number;
  type: string;
  issueDate: string;
  className: string;
  description: string;
}

@Component({
  selector: 'app-summary',
  standalone: true,
  templateUrl: './summary.component.html',
  styleUrl: './summary.component.css'
})
export class SummaryComponent implements OnInit {
  pdfUrl: SafeResourceUrl = '';

  constructor(private sanitizer: DomSanitizer, private http: HttpClient) {}

  ngOnInit(): void {
    const userId = sessionStorage.getItem('userId');
    const userName = sessionStorage.getItem('userName') || 'Nieznany Użytkownik';
    const userClassName = sessionStorage.getItem('userClassName') || 'Nieznana klasa';

    if (userId) {
      this.http.get<GradeSummaryDTO[]>(`${environment.apiUrl}/grades/student/${userId}`)
        .subscribe({
          next: grades => this.generatePDF(grades, userName, userClassName),
          error: err => console.error('Błąd podczas pobierania ocen:', err)
        });
    }
  }

  generatePDF(grades: GradeSummaryDTO[], userName: string, className: string): void {
    const doc = new jsPDF();

    doc.setFontSize(14);
    doc.text(`Imie i nazwisko: ${userName}`, 20, 20);
    // doc.text(`Klasa: ${className}`, 20, 30); ← usunięta linia

    let y = 40;
    const grouped: { [key: string]: number[] } = {};
    grades.forEach(g => {
      if (!grouped[g.className]) grouped[g.className] = [];
      grouped[g.className].push(g.grade);
    });

    Object.entries(grouped).forEach(([subject, grades]) => {
      const avg = grades.reduce((a, b) => a + b, 0) / grades.length;
      doc.text(`${subject} : ${avg.toFixed(2)}`, 20, y);
      y += 10;
    });

    const allGrades = grades.map(g => g.grade);
    const overallAvg = allGrades.length > 0
      ? (allGrades.reduce((a, b) => a + b, 0) / allGrades.length)
      : 0;

    y += 10;
    doc.text(`Srednia wszystkich ocen: ${overallAvg.toFixed(2)}`, 20, y);

    y += 30;
    doc.setFontSize(10);
    doc.text('Podsumowanie zostalo wygenerowane przez aplikacje Memories++', 20, y);

    const blob = doc.output('blob');
    const blobUrl = URL.createObjectURL(blob);
    this.pdfUrl = this.sanitizer.bypassSecurityTrustResourceUrl(blobUrl);
  }
}
