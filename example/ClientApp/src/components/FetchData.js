import React, { Component } from 'react';

export class FetchData extends Component {
	static displayName = FetchData.name;

	constructor(props) {
		super(props);
		this.state = { userInfo: [], loading: true };
	}

	componentDidMount() {
		this.populateWeatherData();
	}

	static renderUserInfoTable(userInfo) {
		return (
			<table className="table table-striped" aria-labelledby="tableLabel">
				<thead>
					<tr>
						<th>Key</th>
						<th>Value</th>
					</tr>
				</thead>
				<tbody>
					{userInfo.map(f =>
						<tr key={f.key}>
							<td>{f.key}</td>
							<td>{f.value}</td>
						</tr>
					)}
				</tbody>
			</table>
		);
	}

	render() {
		let contents = this.state.loading
			? <p><em>Loading...</em></p>
			: FetchData.renderUserInfoTable(this.state.userInfo);

		return (
			<div>
				<h1 id="tableLabel">User Info</h1>
				<p>This component demonstrates fetching data from the server.</p>
				{contents}
			</div>
		);
	}

	async populateWeatherData() {
		const response = await fetch('api/userInfo');
		const data = await response.json();
		this.setState({ userInfo: Object.entries(data).map(d => ({ key: d[0], value: d[1] })), loading: false });
	}
}
