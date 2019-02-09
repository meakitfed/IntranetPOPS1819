class NoteDeFrais extends React.Component {
	constructor(props) {
		super(props);
		this.state = {
			data: { LignesDeFrais: [] }
		};
	}

	componentWillMount() {
		this.loadNoteDeFraisFromServer();
	}
	componentDidMount()
	{
		this.loadNoteDeFraisFromServer();
		window.setInterval(
			() => this.loadNoteDeFraisFromServer(),
			this.props.pollInterval,
		);
	}

	loadNoteDeFraisFromServer() {
		const xhr = new XMLHttpRequest();
		xhr.open('get', this.props.dataUrl, true);
		xhr.onload = () => {
			console.log("Onload : ");
			console.log(xhr.responseText);
			const data = JSON.parse(xhr.responseText);
			this.setState({ data: data});
		};
		xhr.send();
	}

	render()
	{
		var ligne = [];
		this.state.data.LignesDeFrais.map((item) => {
			ligne.push(<LigneDeFrais key={item.Id} item={item} />);
		});
		return (<table className="table table-bordered table-responsive">
			<thead>
				<tr>
					<th>Nom</th>
				</tr>
			</thead>
			<tbody>
				{ligne}
			</tbody>
		</table>);
	}
}

class LigneDeFrais extends React.Component {
	render() {
			return (
				<tr>
					<td>{this.props.item.Nom}</td>
					<td>{this.props.item.Date}</td>
					<td>{this.props.item.Mission.Nom}</td>
					<td>{this.props.item.Somme}</td>
					<td>{this.props.item.Statut}</td>
				</tr>
			);
	}
}

ReactDOM.render(
	<NoteDeFrais dataUrl="/Login/GetNoteDeFrais" pollInterval={2000}/>,
	document.getElementById('ListeNoteDeFrais')
);