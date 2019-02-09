const data = [
    { Id: 1, Author: 'Nathan Bonnard', Text: 'Je  *pete* regulierement sur les gens!' },
    { Id: 2, Author: 'Brian', Text: 'Je suis Brian et je suis musclé' },
    { Id: 3, Author: 'Mahitas', Text: "Salut j'ai fait une faute à mon propre nom" },
];

class CommentList extends React.Component {
    render() {

        const commentNodes = this.props.data.map(comment => (
            <Comment author={comment.Author} key={comment.Id}>
                {comment.Text}
            </Comment>
        ));

        return <div className="commentList">{commentNodes}</div>;
    }
}

class Comment extends React.Component {
    rawMarkup() {
        const md = new Remarkable();
        const rawMarkup = md.render(this.props.children.toString());
        return { __html: rawMarkup };
	}

    render() {
        return (
            <div className="comment">
                <h3 className="commentAuthor">{this.props.author}</h3>
                <span dangerouslySetInnerHTML={this.rawMarkup()} />
            </div>
        );
    }
}

class LoginForm extends React.Component {
    render() {
        return (
            <form className="loginForm">
                <p>Ceci est un login form en react mais pas encore fonctionnel c'est juste pour voir</p>
                <input type="text" placeholder="Identifiant" />
                <input type="text" placeholder="Mot de passe" />
                <input type="submit" value="Post" />
            </form>
        );
    }
}

class CommentBox extends React.Component {
    render() {
        return (
            <div className="commentBox">
                <h1>Comments</h1>
                <CommentList data={this.props.data} />
                <LoginForm />
            </div>
        );
    }
}

ReactDOM.render(<CommentBox data={data} />, document.getElementById('content'));