export function eventMatchesFilter(event, selectedFilter, categoryFilterMap = {}) {
  if (selectedFilter === "Todos") {
    return true;
  }

  const mappedCategories = categoryFilterMap[selectedFilter];

  return (
    event.category === selectedFilter ||
    event.filterTags?.includes(selectedFilter) ||
    mappedCategories?.includes(event.category)
  );
}

export function filterEventsByCategory(
  events,
  selectedFilter,
  categoryFilterMap = {}
) {
  return events.filter((event) =>
    eventMatchesFilter(event, selectedFilter, categoryFilterMap)
  );
}
