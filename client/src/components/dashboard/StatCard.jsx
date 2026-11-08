function StatCard({ label, value, variant = 'primary', hint }) {
  return (
    <div className="card h-100 shadow-sm">
      <div className="card-body">
        <div className="text-body-secondary small text-uppercase fw-semibold">{label}</div>
        <div className={`display-6 fw-semibold text-${variant}`}>{value.toLocaleString()}</div>
        {hint && <div className="small text-body-secondary">{hint}</div>}
      </div>
    </div>
  );
}

export default StatCard;