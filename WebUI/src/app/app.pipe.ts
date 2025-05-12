import { Pipe, PipeTransform } from '@angular/core';
import { ListItem } from './app.model';



@Pipe({
    name: 'multiSelectFilter',
    pure: false
})
export class ListFilterPipe implements PipeTransform {
    transform(items: ListItem[], filter: ListItem): ListItem[] {
        if (!items || !filter) {
            return items;
        }
        return items.filter((item: ListItem) => this.applyFilter(item, filter));
    }

    applyFilter(item: ListItem, filter: ListItem): boolean {
        if (typeof item.text === 'string' && typeof filter.text === 'string') {
            return !(filter.text && item.text && item.text.toLowerCase().indexOf(filter.text.toLowerCase()) === -1);
        } else {
            return !(filter.text && item.text && item.text.toString().toLowerCase().indexOf(filter.text.toString().toLowerCase()) === -1);
        }
    }
}

@Pipe({ name: 'filterByName' })
export class FilterByNamePipe implements PipeTransform {
  transform(items: any[], search: string): any[] {
    if (!items || !search) return items;
    const lower = search.toLowerCase();
    return items.filter(i =>
      (i.commonName?.toLowerCase().includes(lower) ||
        i.botanicalName?.toLowerCase().includes(lower) ||
        i.name?.toLowerCase().includes(lower)) ||
        i?.translations?.some((t:any) => t.translated?.toLowerCase().includes(lower))// for groups
    );
  }
}

@Pipe({ name: 'filterRateTable' })
export class FilterRateTablePipe implements PipeTransform {
  transform(items: any[], search: string): any[] {
    if (!items || !search) return items;
    const lower = search.toLowerCase();
    return items.filter(r =>
      (r.plantName?.toLowerCase().includes(lower) ||
        r.groupName?.toLowerCase().includes(lower))
    );
  }
}