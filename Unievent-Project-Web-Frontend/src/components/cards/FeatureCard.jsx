import { Link } from 'react-router-dom';

export function FeatureCard({ to, image, title, description, className = '' }) {
  return (
    <Link to={to} className={`feature-card ${className}`.trim()}>
      <img src={image} alt="" />
      <span>{title}</span>
      {description ? <small>{description}</small> : null}
    </Link>
  );
}
