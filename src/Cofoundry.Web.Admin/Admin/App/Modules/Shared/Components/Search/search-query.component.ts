import { Location } from '@angular/common';
import * as _ from 'lodash';

export class SearchQuery {
	useHistory: boolean;
	filters;
	searchParams;
	defaultParams;
	options;
	pagingDefaultConfig = {
		pageSize: 20,
		pageNumber: 1
	};

	/**
	 * Represents a query for searching entities and returning a paged result, handling
	 * the persistance of the query parameters in the query string.
	 */
	constructor(options: any) {
		this.options = options;
		this.useHistory = _.isUndefined(options.useHistory) || options.useHistory;
		this.defaultParams = _.defaults({}, options.defaultParams, this.pagingDefaultConfig);
		this.filters = options.filters || [];
		this.searchParams = _.defaults({}, this.parseSearchParams(), this.defaultParams);
	}

	/* Public Funcs */

	/**
	 * Gets an object containing all the query params including filters and 
	 * paging parameters.
	 */
	getParameters() {
		return this.searchParams;
	}

	/**
	 * Gets an object containing the filter portion of the query params.
	 */
	getFilters() {
		return _.omit(this.searchParams, Object.keys(this.pagingDefaultConfig));
	}

	/**
	 * Updates the query parameters.
	 */
	update(query) {
		var newParams = _.defaults({}, query, this.pagingDefaultConfig, this.searchParams);
		this.setParams(newParams);
	}

	/**
	 * Resets the query to the defaul parameters
	 */
	clear() {
		this.setParams(this.defaultParams);
	}

	/* Private Funcs */

	parseSearchParams() {
		if (!this.useHistory) return {};

		var params = {}; //this.location.search();

		_.each(params, (value, key) => {
			// parse numbers
			//if (!isNaN(value)) {
			//    params[key] = parseInt(value);
			//}
		});

		return params;
	}

	setParams(params) {
		this.searchParams = params;

		if (this.useHistory) {
			// filter out default params so they dont appear in the query string
			let qsParams = _.omit(params, (value, key) => {
				return this.defaultParams[key] === value || !value;
			});

			//$location.search(qsParams);
		}

		if (this.options.onChanged) this.options.onChanged(this);
	}
}
